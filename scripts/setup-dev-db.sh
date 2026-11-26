#!/usr/bin/env bash
# 開発・動作確認用のデータベースを用意する。
#
#   scripts/setup-dev-db.sh           データベースがなければ作り、テスト用ユーザーを追加する（既存のデータは残す）
#   scripts/setup-dev-db.sh --reset   データベースを削除して、初期状態から作り直す
#
# 作成先: src/TaskDoc.Api/App_Data/taskdoc.db（git の管理対象外）
# 中身は API 自身が作る（テーブルは DbUp のマイグレーション、パスワードは本番と同じハッシュ方式）。
# Windows などでは、同じことを次のコマンドで実行できる:
#   dotnet run --project src/TaskDoc.Api --launch-profile http -- seed-dev [--reset]
#
# 作成されるユーザー（開発専用。既知のパスワードなので本番では使わないこと）:
#   admin   / Admin-pass-1   システムユーザー・管理者
#   yamada  / User-pass-1    システムユーザー
#   sato    / （AD）          AD ユーザー。AD 未設定のためログインすると「認証サーバーに接続できません」になる
#   suzuki  / User-pass-1    システムユーザー・無効（ログインできないことの確認用）

set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ARGS=(seed-dev)

case "${1:-}" in
    "") ;;
    --reset) ARGS+=(--reset) ;;
    -h|--help) sed -n '2,17p' "${BASH_SOURCE[0]}" | sed 's/^# \{0,1\}//'; exit 0 ;;
    *) echo "不明な引数: $1（--reset または --help）" >&2; exit 2 ;;
esac

if ! command -v dotnet >/dev/null 2>&1; then
    echo ".NET SDK（dotnet）が見つかりません。.NET 8 SDK をインストールしてください。" >&2
    exit 2
fi

cd "$ROOT"

if ss -ltn 2>/dev/null | grep -q ':5227 '; then
    echo "注意: API（ポート 5227）が起動中です。--reset で作り直す場合は、先に API を止めてください。" >&2
    [[ " ${ARGS[*]} " == *" --reset "* ]] && exit 1
fi

echo "== API をビルドしてデータベースを準備します"
# launch profile "http" sets ASPNETCORE_ENVIRONMENT=Development; seed-dev refuses to run otherwise.
dotnet run --project src/TaskDoc.Api --launch-profile http -- "${ARGS[@]}"

cat <<'EOF'

データベース: src/TaskDoc.Api/App_Data/taskdoc.db

ログインできるユーザー（開発専用）:
  admin  / Admin-pass-1   管理者
  yamada / User-pass-1    一般ユーザー
  sato   （AD ユーザー。AD 未設定のためログイン不可）
  suzuki （無効なユーザー。ログイン不可）

次に起動するもの:
  API:          dotnet run --project src/TaskDoc.Api --launch-profile http
  デスクトップ:  dotnet run --project src/TaskDoc.App.Desktop
EOF
