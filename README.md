# TaskDoc

タスクとドキュメントを紐づけて管理するアプリケーション。仕様は [docs/spec.md](docs/spec.md)、API は [docs/api.md](docs/api.md) を参照。
クローン直後のセットアップ（DB の初期化・初期管理者など）は [docs/setup.md](docs/setup.md) を参照。

## 構成

| パス | 内容 |
|---|---|
| `src/TaskDoc.Contracts` | API とクライアントで共有する DTO |
| `src/TaskDoc.Api` | ASP.NET Core Minimal API（Dapper + SQLite、DbUp でスキーマ管理） |
| `src/TaskDoc.App` | 共通 UI（Avalonia 12 / [ShadUI](https://github.com/accntech/shad-ui) / CommunityToolkit.Mvvm / Refit） |
| `src/TaskDoc.App.Desktop` | Windows / macOS / Linux |
| `src/TaskDoc.App.Browser` | WebAssembly |
| `src/TaskDoc.App.Android` | Android（JDK と Android SDK が必要） |
| `src/TaskDoc.App.iOS` | iOS（macOS + Xcode が必要） |
| `tests/TaskDoc.Api.Tests` | API の結合テスト（一時 SQLite ファイルを使用） |
| `tests/TaskDoc.App.Tests` | UI テスト（Avalonia Headless） |

- NuGet のバージョンは `Directory.Packages.props` で一括管理する（csproj には Version を書かない）。
- `TaskDoc.slnx` は全プロジェクト、`TaskDoc.Dev.slnf` は Android / iOS を除いた開発用のソリューションフィルター。

## 開発

```bash
# ビルドとテスト（Linux では Android / iOS を除く）
dotnet build TaskDoc.Dev.slnf
dotnet test --solution TaskDoc.Dev.slnf

# API を起動（http://localhost:5227、DB は src/TaskDoc.Api/App_Data/taskdoc.db に作成）
dotnet run --project src/TaskDoc.Api --launch-profile http

# デスクトップアプリを起動
dotnet run --project src/TaskDoc.App.Desktop

# ブラウザ版を起動（http://localhost:5235）
dotnet run --project src/TaskDoc.App.Browser
```

## UI（ShadUI）

- テーマは `App.axaml` の `<shadui:ShadTheme />`。FluentTheme は使っていない。
- 色は ShadUI のキーを `DynamicResource` で使う（`ForegroundColor` `MutedColor` `BorderColor` `PrimaryColor` `CardBackgroundColor` `ErrorColor` など）。存在しないキーは何も表示されないまま失敗するため、`AppResourceKeysTests` が XAML 中の全キーをライト/ダーク両方で確認している。
- ボタンは `Classes="Primary"` / `Outline` / `Ghost` / `Destructive`、入力欄のラベルは `shadui:ControlAssist.Label`、カードは `shadui:Card`。
- アイコンは Lucide フォント（`Assets/Fonts/lucide.ttf`、ISC ライセンス、`LucideIcons.cs` にコードポイント）。`<TextBlock Classes="icon" Text="{x:Static app:LucideIcons.House}" />`。
- ダイアログとトーストは `MainView` の `DialogHost` / `ToastHost` に表示される（デスクトップ・ブラウザ・モバイル共通）。独自ビューのダイアログは `ServiceCollectionExtensions` で `DialogManager.Register<View, ViewModel>()` する。
- ShadUI のフォントは Inter が前提。各プラットフォームの起動処理で `.WithInterFont()` を外さないこと（外すと起動時に例外）。

## テストの実行

```bash
scripts/test.sh                  # ビルドしてすべてのテストを実行（終了コード 0 なら成功）
scripts/test.sh --no-build       # ビルドを省略
scripts/test.sh api              # API のテストだけ（app でアプリだけ）
scripts/test.sh app -- -class TaskDoc.App.Tests.UsersViewTests   # "--" 以降は xUnit にそのまま渡す
```

`dotnet test` は、テストの準備や後片付けで起きたエラー（xUnit の "Test Case Cleanup Failure" など）を「成功」にも「失敗」にも数えず、全体を成功と表示することがある。
`scripts/test.sh` はテストの実行ファイルを直接動かし、失敗・エラー・実行 0 件のどれかがあれば失敗（終了コード 1）にする。ふだんはこちらを使う。

## 認証

- **初回起動**: ユーザーが 1 人もいなければ、API がシステムユーザー `admin` を作成し、ランダムなパスワードをログに 1 回だけ出力する（`Created initial admin 'admin' with password '...'`）。ログイン後に変更すること。
  - パスワードを自分で決めたいときは、DB が空の状態で環境変数 `Auth__InitialAdmin__Password`（8 文字以上）を設定して起動する。この場合パスワードはログに出ない。8 文字未満だと起動に失敗する。ユーザー名は `Auth__InitialAdmin__UserName` で変えられる。

    ```powershell
    $env:Auth__InitialAdmin__Password = "任意のパスワード"
    dotnet run --project src/TaskDoc.Api --launch-profile http
    ```
- **署名鍵**: 開発環境では未設定でも動く（API を再起動するとアクセストークンは無効になるが、クライアントがリフレッシュトークンで自動的に取り直す）。本番では環境変数 `Auth__Jwt__SigningKey`（32 文字以上）を設定しないと起動しない。
- **Active Directory**: `appsettings.json` の `Auth:ActiveDirectory` で `Enabled`、`Server`、`Domain` を設定する。Linux で動かす場合は OpenLDAP クライアント（`libldap`）と、LDAPS 用にドメインの CA 証明書が必要。
- 認証まわりの仕様は [docs/spec.md](docs/spec.md) の 3 章を参照。

## データベースのスキーマ変更

`src/TaskDoc.Api/Data/Migrations/` に番号付きの SQL ファイル（`0002_xxx.sql` など）を追加する。
API の起動時に未適用のものが番号順に実行される。適用済みのファイルは書き換えず、変更は新しいファイルで行う。
