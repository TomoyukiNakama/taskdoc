# セットアップ手順

リポジトリをクローンしてから、API とアプリを動かすまでの手順。
プロジェクトの構成や日常の開発コマンドは [README](../README.md) を参照。

## 1. 必要なもの

| もの | 用途 | 備考 |
|---|---|---|
| .NET SDK 8.0.100 以上（8.0.x） | API、デスクトップ版、WPF 版、Windows Forms 版、テスト | ルートの `global.json` で指定。`dotnet --version` で確認する |
| .NET SDK 10.0.100 以上 | ブラウザ版、Android 版、iOS 版 | Avalonia 12 がこれらに net10.0 を必須としているため。各フォルダーの `global.json` で指定 |
| `wasm-tools` ワークロード | ブラウザ版（`TaskDoc.App.Browser`） | Avalonia のブラウザ版で推奨されている。`dotnet workload install wasm-tools` |
| `android` ワークロード、JDK、Android SDK | Android 版 | Android 版をビルドするときだけ |
| macOS、Xcode、`ios` ワークロード | iOS 版 | iOS 版をビルドするときだけ |
| OpenLDAP クライアント（`libldap`） | Linux で Active Directory 認証を使うとき | Windows / macOS では不要 |

SQLite は NuGet パッケージに含まれているので、別途インストールしなくてよい。

## 2. クローンとビルド

```powershell
git clone https://github.com/TomoyukiNakama/taskdoc.git
cd taskdoc
dotnet build TaskDoc.sln
```

`TaskDoc.sln` には .NET 8 SDK でビルドできるプロジェクトだけが入っている。
ブラウザ版・Android 版・iOS 版は .NET 10 SDK が必要なので、それぞれのフォルダーに移動してからビルドする
（フォルダー内の `global.json` で SDK 10 が選ばれる。リポジトリのルートから `--project` で指定すると SDK 8 が選ばれて失敗する）。

```powershell
cd src/TaskDoc.App.Browser   # または src/TaskDoc.App.Android / src/TaskDoc.App.iOS
dotnet build
```

## 3. データベースの初期化

DB を作るためのコマンドは不要。**API を初めて起動したとき**に、次の処理が自動で行われる。

1. SQLite ファイル `src/TaskDoc.Api/App_Data/taskdoc.db` を作る（フォルダごと）。
2. `src/TaskDoc.Api/Data/Migrations/` の SQL を番号順に適用する。
3. ユーザーが 1 人もいなければ、初期管理者 `admin` を作る。

### 3-1. 初期管理者のパスワードを決める（任意）

何も設定しないと、ランダムなパスワードが作られ、起動ログに **1 回だけ** 出力される。

```text
warn: InitialAdminSeeder[0]
      Created initial admin 'admin' with password 'xxxxxxxxxxxxxxxx'. Sign in and change this password now; it is shown only once.
```

見逃すと再表示できないので、自分で決めたい場合は、初回起動の前に環境変数で指定する。

```powershell
# PowerShell
$env:Auth__InitialAdmin__Password = "8文字以上のパスワード"
$env:Auth__InitialAdmin__UserName = "admin"   # 変えたいときだけ
```

```bash
# bash
export Auth__InitialAdmin__Password='8文字以上のパスワード'
```

- 指定したパスワードはログに出ない。
- 8 文字未満だと API は起動に失敗する。
- この設定が使われるのは **DB にユーザーがいないとき** だけ。2 回目以降の起動では無視される。
- パスワードを `appsettings.json` に書いてコミットしないこと。

### 3-2. API を起動する

```powershell
dotnet run --project src/TaskDoc.Api --launch-profile http
```

次のログが出れば初期化は完了している。

```text
info: TaskDoc.Api[0]
      Applied migration TaskDoc.Api.Data.Migrations.0001_Initialize.sql
info: TaskDoc.Api[0]
      Applied migration TaskDoc.Api.Data.Migrations.0002_Users.sql
warn: InitialAdminSeeder[0]
      Created initial admin 'admin' ...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5227
```

動作確認：

```powershell
curl http://localhost:5227/api/health
# {"status":"ok","databaseVersion":"...","serverTime":"..."}
```

開発環境では `Auth:Jwt:SigningKey` を設定しなくても動く（`SigningKey is not set; using a temporary development key.` という警告が出るが問題ない）。

## 4. アプリを起動する

API を起動したまま、別のターミナルで起動する。

```powershell
# デスクトップ版（Windows / macOS / Linux）
dotnet run --project src/TaskDoc.App.Desktop

# ブラウザ版（http://localhost:5235 を開く。.NET 10 SDK が必要なので、フォルダーに移動してから実行する）
cd src/TaskDoc.App.Browser
dotnet run
```

手順 3 の初期管理者でログインし、**最初にパスワードを変更する**。
ほかのユーザーは、管理者がアプリのユーザー管理画面から登録する（登録されていないユーザーはログインできない）。

アプリの接続先は既定で `http://localhost:5227`（Android エミュレーターでは `http://10.0.2.2:5227`）。
API のポートを変えた場合は `src/TaskDoc.App/Services/ClientOptions.cs` の `ApiBaseAddress` も合わせる。

## 5. テストを実行する

```bash
scripts/test.sh
```

Windows では Git Bash から実行する。テストは一時ファイルの SQLite を使うので、手順 3 の DB には影響しない。

**Windows の注意**: API やアプリを起動したままビルド・テストすると、実行中の DLL がロックされてビルドに失敗する。先に止めておくこと。

## 6. データベースを作り直す

API を止めてから DB ファイルを消し、もう一度起動する。初期管理者も作り直される（パスワードは手順 3-1 のとおり）。

```powershell
Remove-Item src/TaskDoc.Api/App_Data/taskdoc.db*
dotnet run --project src/TaskDoc.Api --launch-profile http
```

`taskdoc.db-shm` / `taskdoc.db-wal` が残っていれば一緒に消す（上のコマンドはまとめて消す）。

## 7. 必要に応じて設定するもの

設定は `src/TaskDoc.Api/appsettings.json` にあり、環境変数で上書きできる（`:` を `__` に置き換える。例：`Auth:Jwt:SigningKey` → `Auth__Jwt__SigningKey`）。
パスワードや鍵などの秘密の値は、ファイルに書かずに環境変数で渡す。

| 設定 | 既定値 | 説明 |
|---|---|---|
| `ConnectionStrings:Default` | `Data Source=App_Data/taskdoc.db` | DB ファイルの場所。相対パスは API の作業ディレクトリが基準（`dotnet run --project` では API プロジェクトのフォルダ） |
| `Auth:InitialAdmin:UserName` | `admin` | 初期管理者のユーザー名 |
| `Auth:InitialAdmin:Password` | なし（ランダム） | 初期管理者のパスワード（手順 3-1） |
| `Auth:Jwt:SigningKey` | なし | トークンの署名鍵（32 文字以上）。**本番では必須**で、未設定だと起動しない |
| `Auth:ActiveDirectory:Enabled` / `Server` / `Domain` | 無効 | Active Directory 認証を使うとき（仕様は [spec.md](spec.md) の 3 章） |
| `Cors:AllowedOrigins` | 開発環境ではブラウザ版の URL | ブラウザ版を別のオリジンから配信するとき |

本番環境の例（PowerShell）：

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:Auth__Jwt__SigningKey = "<32文字以上のランダムな文字列>"
$env:Auth__InitialAdmin__Password = "<初期管理者のパスワード>"
```
