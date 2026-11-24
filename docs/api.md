# TaskDoc API リファレンス

TaskDoc.Api（ASP.NET Core Minimal API）のエンドポイント一覧です。
認証やユーザー管理のルール（なぜそう動くか）は [spec.md](spec.md) の 3 章を参照してください。

- 開発時のベース URL: `http://localhost:5227`（`src/TaskDoc.Api/Properties/launchSettings.json` の `http` プロファイル）
- 開発環境では OpenAPI 定義を `GET /openapi/v1.json` で取得できます。ただし 401 / 409 / 503 のエラー応答はそこに含まれないため、この文書を正としてください。
- リクエスト・レスポンスの型は `src/TaskDoc.Contracts` にあり、クライアント（`TaskDoc.App`）と共有しています。

## 目次

| メソッド | パス | 認証 | 概要 |
|---|---|---|---|
| GET | [`/api/health`](#get-apihealth) | 不要 | 稼働確認 |
| POST | [`/api/auth/login`](#post-apiauthlogin) | 不要 | ログイン（トークン発行） |
| POST | [`/api/auth/refresh`](#post-apiauthrefresh) | 不要 | トークン更新 |
| POST | [`/api/auth/logout`](#post-apiauthlogout) | 不要 | ログアウト |
| GET | [`/api/auth/me`](#get-apiauthme) | 必要 | ログイン中のユーザー |
| PUT | [`/api/auth/me/password`](#put-apiauthmepassword) | 必要 | 自分のパスワード変更 |
| GET | [`/api/users`](#get-apiusers) | 管理者 | ユーザー一覧 |
| GET | [`/api/users/{id}`](#get-apiusersid) | 管理者 | ユーザー取得 |
| POST | [`/api/users`](#post-apiusers) | 管理者 | ユーザー登録 |
| PUT | [`/api/users/{id}`](#put-apiusersid) | 管理者 | ユーザー更新 |
| PUT | [`/api/users/{id}/password`](#put-apiusersidpassword) | 管理者 | パスワード再設定 |

---

## 共通事項

### 認証

ログインで受け取った `accessToken` を `Authorization` ヘッダーに付けて送ります。

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

| トークン | 有効期限 | 使い方 |
|---|---|---|
| アクセストークン（JWT） | 15 分 | API 呼び出しのたびに `Authorization` ヘッダーで送る |
| リフレッシュトークン | 14 日 | アクセストークンが切れる前に [`/api/auth/refresh`](#post-apiauthrefresh) へ送り、新しいトークンの組を受け取る |

- リフレッシュトークンは **1 回しか使えません**。更新すると新しいものが返り、古いものは無効になります。
- 無効になったリフレッシュトークンが再び送られると、盗用とみなして **そのユーザーのすべてのセッションを無効**にします。同じトークンで同時に更新しないよう、クライアント側で更新を直列化してください（`TaskDoc.App` の `AuthSession` が行っています）。
- 「認証: 必要」のエンドポイントにトークンなし、または期限切れのトークンで来た場合は `401`（本文なし、`WWW-Authenticate: Bearer`）を返します。
- 「認証: 管理者」のエンドポイントに管理者でないユーザーが来た場合は `403`（本文なし）を返します。

アクセストークン（JWT）に含まれるクレーム:

| クレーム | 内容 |
|---|---|
| `sub` | ユーザー ID |
| `name` | ユーザー名 |
| `display_name` | 表示名 |
| `role` | 管理者のときだけ `admin` |
| `jti` | トークン ID |
| `iss` / `aud` | `TaskDoc`（設定 `Auth:Jwt:Issuer` / `Audience`） |

### JSON の形式

- プロパティ名は **camelCase**（例: `userName`、`displayName`）。
- 日時は ISO 8601 形式の文字列（例: `"2026-11-24T12:04:39.1635401+00:00"`）。API は UTC で返します。
- `authType` は文字列 `"Local"`（システムユーザー）または `"Ad"`（AD ユーザー）。

### エラー応答

エラーの本文は [RFC 9457](https://www.rfc-editor.org/rfc/rfc9457)（Problem Details）形式で、`Content-Type: application/problem+json` です。
`detail` と `errors` のメッセージは日本語で、**そのまま画面に表示できる文言**にしています。

入力エラー（`400`）の例。`errors` には項目名ごとのメッセージが入ります。

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "UserName": ["ユーザー名は半角英数字と . _ - で 1〜64 文字にしてください。"],
    "Password": ["パスワードは8文字以上にしてください。"]
  },
  "traceId": "00-a94f383dec9a4b0eab860efd7f7bb597-2bd5438f398eca7e-00"
}
```

`traceId` はサーバーログとの突き合わせ用です（問い合わせ時に伝えてもらうと調査しやすくなります）。

それ以外のエラー（`401` / `409` / `503` など）の例。

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "detail": "ユーザー名またはパスワードが正しくありません。",
  "traceId": "00-57a22500fbaf0c40c1ff6817d0225c9c-1db491beda19fe38-00"
}
```

### CORS

ブラウザ版（WASM）は別オリジンから呼ぶため、許可するオリジンを設定 `Cors:AllowedOrigins` に列挙します。開発環境では `http://localhost:5235` と `https://localhost:7169` を許可しています。

---

## 型

### UserDto

| プロパティ | 型 | 説明 |
|---|---|---|
| `id` | number | ユーザー ID |
| `userName` | string | ユーザー名（大文字小文字を区別しない）。AD ユーザーは AD のアカウント名（sAMAccountName） |
| `displayName` | string | 表示名 |
| `email` | string \| null | メールアドレス |
| `authType` | `"Local"` \| `"Ad"` | 認証の種類 |
| `isAdmin` | boolean | システム管理者か |
| `isActive` | boolean | 有効か（`false` だとログインできない） |
| `createdAt` | string（日時） | 登録日時 |
| `lastLoginAt` | string（日時） \| null | 最終ログイン日時。一度もログインしていなければ `null` |

```json
{
  "id": 2,
  "userName": "yamada",
  "displayName": "山田 太郎",
  "email": "yamada@example.com",
  "authType": "Local",
  "isAdmin": false,
  "isActive": true,
  "createdAt": "2026-11-24T12:10:00+00:00",
  "lastLoginAt": null
}
```

### TokenResponse

ログイン・トークン更新・自分のパスワード変更で返ります。

| プロパティ | 型 | 説明 |
|---|---|---|
| `accessToken` | string | アクセストークン（JWT） |
| `accessTokenExpiresAt` | string（日時） | アクセストークンの有効期限 |
| `refreshToken` | string | リフレッシュトークン |
| `refreshTokenExpiresAt` | string（日時） | リフレッシュトークンの有効期限 |
| `user` | [UserDto](#userdto) | ログインしたユーザー |

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "accessTokenExpiresAt": "2026-11-24T12:25:00+00:00",
  "refreshToken": "q3Xv9...",
  "refreshTokenExpiresAt": "2026-12-08T12:10:00+00:00",
  "user": { "id": 1, "userName": "admin", "displayName": "管理者", "email": null, "authType": "Local", "isAdmin": true, "isActive": true, "createdAt": "2026-11-24T12:04:00+00:00", "lastLoginAt": "2026-11-24T12:10:00+00:00" }
}
```

---

## ヘルスチェック

### GET /api/health

API とデータベースに接続できるかを確認します。認証不要。

**レスポンス `200`**

| プロパティ | 型 | 説明 |
|---|---|---|
| `status` | string | 常に `"ok"` |
| `databaseVersion` | string | SQLite のバージョン |
| `serverTime` | string（日時） | サーバーの現在時刻（UTC） |

```json
{ "status": "ok", "databaseVersion": "3.53.3", "serverTime": "2026-11-24T12:04:39.1635401+00:00" }
```

---

## 認証

### POST /api/auth/login

ユーザー名とパスワードでログインし、トークンを発行します。システムユーザーと AD ユーザーのどちらも同じエンドポイントです（API がユーザーの種類を見て、DB または AD で確認します）。

- `userName` は `user`、`DOMAIN\user`、`user@domain` のどの形でも受け付け、`user` として扱います。
- **DB に登録されていないユーザーはログインできません。** AD で認証できても、登録がなければ拒否します。
- 5 回続けて失敗すると、そのユーザーは 15 分間ロックされます（AD ユーザーも同じ）。

**リクエスト**

| プロパティ | 型 | 必須 |
|---|---|---|
| `userName` | string | ○ |
| `password` | string | ○ |

```http
POST /api/auth/login
Content-Type: application/json

{ "userName": "admin", "password": "your-password" }
```

**レスポンス**

| ステータス | 本文 | いつ |
|---|---|---|
| `200` | [TokenResponse](#tokenresponse) | 成功 |
| `401` | Problem（`detail`: 「ユーザー名またはパスワードが正しくありません。」） | ユーザーがいない、パスワード違い、無効なユーザー、ロック中。どの理由かは返しません（ユーザー名の存在を推測されないため） |
| `503` | Problem（`detail`: 「認証サーバー（Active Directory）に接続できません。…」） | AD ユーザーで、AD に接続できない、または AD 連携が無効（`Auth:ActiveDirectory:Enabled=false`） |

### POST /api/auth/refresh

リフレッシュトークンを新しいトークンの組と交換します。認証ヘッダーは不要です（リフレッシュトークン自体が証明になります）。

**リクエスト**

```json
{ "refreshToken": "q3Xv9..." }
```

**レスポンス**

| ステータス | 本文 | いつ |
|---|---|---|
| `200` | [TokenResponse](#tokenresponse) | 成功。送ったリフレッシュトークンはこの時点で無効になる |
| `401` | Problem（`detail`: 「セッションの有効期限が切れました。再度ログインしてください。」） | 不明・期限切れ・無効化済みのトークン、またはユーザーが無効化された。無効化済みトークンの再利用だった場合は、そのユーザーの全セッションも無効にする |

### POST /api/auth/logout

リフレッシュトークンを無効にします。認証ヘッダーは不要です。

```json
{ "refreshToken": "q3Xv9..." }
```

**レスポンス** `204`（本文なし）。トークンが不明でも `204` を返します。

発行済みのアクセストークンは、期限（最大 15 分）までは使えます。クライアントは手元のトークンを破棄してください。

### GET /api/auth/me

ログイン中のユーザーを返します。**認証: 必要**

**レスポンス**

| ステータス | 本文 | いつ |
|---|---|---|
| `200` | [UserDto](#userdto) | 成功 |
| `401` | なし | トークンがない・期限切れ、またはユーザーが無効化された |

### PUT /api/auth/me/password

自分のパスワードを変更します。**認証: 必要**。システムユーザーのみ（AD ユーザーのパスワードは AD で変更します）。

変更すると **そのユーザーのすべてのセッション（ほかの端末を含む）を無効**にし、呼び出した端末用の新しいトークンを返します。クライアントは返ってきたトークンに切り替えてください。

**リクエスト**

| プロパティ | 型 | 必須 | 制約 |
|---|---|---|---|
| `currentPassword` | string | ○ | 現在のパスワード |
| `newPassword` | string | ○ | 8 文字以上 |

```json
{ "currentPassword": "old-password", "newPassword": "new-password-1" }
```

**レスポンス**

| ステータス | 本文 | いつ |
|---|---|---|
| `200` | [TokenResponse](#tokenresponse) | 成功（新しいセッション） |
| `400` | 入力エラー（`errors.CurrentPassword` または `errors.NewPassword`） | 現在のパスワードが違う、新しいパスワードが短い |
| `400` | Problem（`detail`: 「AD ユーザーのパスワードは Active Directory で変更してください。」） | AD ユーザーが呼んだ |
| `401` | なし | 未認証、またはユーザーが無効化された |

---

## ユーザー管理

すべて **認証: 管理者**（アクセストークンの `role` が `admin`）。管理者でなければ `403` を返します。

### GET /api/users

全ユーザーをユーザー名順で返します。

**レスポンス `200`**: [UserDto](#userdto) の配列

### GET /api/users/{id}

| ステータス | 本文 |
|---|---|
| `200` | [UserDto](#userdto) |
| `404` | なし |

### POST /api/users

ユーザーを登録します。登録したユーザーはすぐにログインできます。

**リクエスト**

| プロパティ | 型 | 必須 | 制約 |
|---|---|---|---|
| `userName` | string | ○ | 半角英数字と `. _ -` で 1〜64 文字。既存のユーザー名と重複不可（大文字小文字を区別しない） |
| `displayName` | string | ○ | 1〜100 文字 |
| `email` | string \| null | | 256 文字以内、`@` を含む |
| `authType` | `"Local"` \| `"Ad"` | ○ | 登録後は変更不可 |
| `password` | string \| null | Local のとき ○ | Local: 8 文字以上。Ad: `null` にする（AD のパスワードでログインするため） |
| `isAdmin` | boolean | ○ | |

```json
{
  "userName": "yamada",
  "displayName": "山田 太郎",
  "email": "yamada@example.com",
  "authType": "Local",
  "password": "initial-pass-1",
  "isAdmin": false
}
```

AD ユーザーの場合:

```json
{ "userName": "sato", "displayName": "佐藤 花子", "email": null, "authType": "Ad", "password": null, "isAdmin": false }
```

**レスポンス**

| ステータス | 本文 | いつ |
|---|---|---|
| `201` | [UserDto](#userdto)（`Location: /api/users/{id}`） | 成功 |
| `400` | 入力エラー（`errors.UserName` / `DisplayName` / `Email` / `Password`） | 制約違反 |
| `409` | Problem（`detail`: 「ユーザー名 'xxx' は既に登録されています。」） | ユーザー名の重複 |

### PUT /api/users/{id}

表示名・メールアドレス・管理者フラグ・有効/無効を変更します。ユーザー名と認証の種類は変更できません。

**リクエスト**（すべて必須。変えない項目も現在の値を送る）

| プロパティ | 型 | 制約 |
|---|---|---|
| `displayName` | string | 1〜100 文字 |
| `email` | string \| null | 256 文字以内、`@` を含む |
| `isAdmin` | boolean | |
| `isActive` | boolean | `false` にするとログイン不可になり、リフレッシュトークンをすべて無効にする |

```json
{ "displayName": "山田 太郎", "email": null, "isAdmin": false, "isActive": false }
```

**レスポンス**

| ステータス | 本文 | いつ |
|---|---|---|
| `200` | [UserDto](#userdto)（更新後） | 成功 |
| `400` | 入力エラー（`errors.DisplayName` / `Email`） | 制約違反 |
| `404` | なし | ユーザーがいない |
| `409` | Problem（`detail`: 「有効な管理者が 1 人もいなくなるため、この変更はできません。」） | 最後の有効な管理者から管理者権限を外す、または無効にしようとした |

無効にしたユーザーのアクセストークンは、期限（最大 15 分）までは使えます。

### PUT /api/users/{id}/password

システムユーザーのパスワードを再設定します。そのユーザーのセッションをすべて無効にし、ロックも解除します。

**リクエスト**

```json
{ "newPassword": "reset-pass-1" }
```

| プロパティ | 型 | 必須 | 制約 |
|---|---|---|---|
| `newPassword` | string | ○ | 8 文字以上 |

**レスポンス**

| ステータス | 本文 | いつ |
|---|---|---|
| `204` | なし | 成功 |
| `400` | 入力エラー（`errors.NewPassword`） | 短すぎる |
| `400` | Problem（`detail`: 「AD ユーザーのパスワードは Active Directory で管理されています。」） | AD ユーザーを指定した |
| `404` | なし | ユーザーがいない |

---

## curl での例

```bash
BASE=http://localhost:5227

# ログインしてアクセストークンを取り出す
TOKEN=$(curl -s -X POST $BASE/api/auth/login -H 'Content-Type: application/json' \
  -d '{"userName":"admin","password":"your-password"}' \
  | python3 -c 'import json,sys;print(json.load(sys.stdin)["accessToken"])')

# ユーザー一覧
curl -s $BASE/api/users -H "Authorization: Bearer $TOKEN"

# システムユーザーを登録
curl -s -X POST $BASE/api/users -H "Authorization: Bearer $TOKEN" -H 'Content-Type: application/json' \
  -d '{"userName":"yamada","displayName":"山田 太郎","email":null,"authType":"Local","password":"initial-pass-1","isAdmin":false}'
```

`src/TaskDoc.Api/TaskDoc.Api.http` からも、IDE（Visual Studio / Rider / VS Code の REST Client）で同じように呼べます。

---

## エンドポイントを追加するとき

1. リクエスト・レスポンスの型を `src/TaskDoc.Contracts` に追加し、`TaskDocJsonContext` に `[JsonSerializable(typeof(...))]` を足す（クライアントの Refit が型情報を必要とするため）。
2. `src/TaskDoc.Api/Endpoints` にエンドポイントを追加する。認証不要なら `.AllowAnonymous()` を付ける（付けなければログイン必須になる）。
3. クライアントの `src/TaskDoc.App/Services/ITaskDocApi.cs` にメソッドを追加する。
4. この文書を更新する。
