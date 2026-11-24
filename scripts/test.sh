#!/usr/bin/env bash
# TaskDoc のテストを実行する。
#
#   scripts/test.sh                  ビルドしてすべてのテストを実行
#   scripts/test.sh --no-build       ビルドを省略（直前にビルド済みのとき）
#   scripts/test.sh api              API のテストだけ
#   scripts/test.sh app              アプリ（UI）のテストだけ
#   scripts/test.sh app -- -class TaskDoc.App.Tests.UsersViewTests
#                                    "--" 以降は xUnit の実行ファイルにそのまま渡す（-class / -method で絞り込み）
#
# `dotnet test` ではなく、テストの実行ファイルを直接動かす。
# `dotnet test` は、テストの準備や後片付けで起きたエラー（xUnit の "Test Case Cleanup Failure" など）を
# 成功にも失敗にも数えずに「成功」と表示することがあるため。ここでは Failed と Errors の両方を見て、
# どちらかが 1 以上、またはテストが 1 件も動かなかったら失敗にする。
#
# 終了コード: 0 = すべて成功、1 = 失敗あり、2 = 使い方の誤り / ビルド失敗

set -uo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIGURATION="Debug"
BUILD=1
SUITES=()
EXTRA_ARGS=()

usage() {
    sed -n '2,10p' "${BASH_SOURCE[0]}" | sed 's/^# \{0,1\}//'
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        --no-build) BUILD=0 ;;
        -c|--configuration) CONFIGURATION="${2:?--configuration には Debug か Release を指定}"; shift ;;
        api|app) SUITES+=("$1") ;;
        -h|--help) usage; exit 0 ;;
        --) shift; EXTRA_ARGS=("$@"); break ;;
        *) echo "不明な引数: $1" >&2; usage >&2; exit 2 ;;
    esac
    shift
done

[[ ${#SUITES[@]} -eq 0 ]] && SUITES=(api app)

if [[ -t 1 ]]; then
    RED=$'\e[31m'; GREEN=$'\e[32m'; BOLD=$'\e[1m'; RESET=$'\e[0m'
else
    RED=""; GREEN=""; BOLD=""; RESET=""
fi

cd "$ROOT"

if [[ $BUILD -eq 1 ]]; then
    echo "${BOLD}== ビルド ($CONFIGURATION, TaskDoc.Dev.slnf)${RESET}"
    if ! dotnet build TaskDoc.Dev.slnf -c "$CONFIGURATION" -nologo -v q; then
        echo "${RED}ビルドに失敗しました。${RESET}" >&2
        exit 2
    fi
fi

overall=0
summary=()

for suite in "${SUITES[@]}"; do
    case "$suite" in
        api) project="TaskDoc.Api.Tests" ;;
        app) project="TaskDoc.App.Tests" ;;
    esac

    dll="tests/$project/bin/$CONFIGURATION/net10.0/$project.dll"
    if [[ ! -f "$dll" ]]; then
        echo "${RED}$dll がありません。--no-build を外してビルドしてください。${RESET}" >&2
        exit 2
    fi

    echo
    echo "${BOLD}== $project${RESET}"
    log="$(mktemp)"
    dotnet "$dll" -noColor "${EXTRA_ARGS[@]}" 2>&1 | tee "$log"
    exe_status=${PIPESTATUS[0]}

    # 例: "TaskDoc.App.Tests  Total: 28, Errors: 0, Failed: 0, Skipped: 0, Not Run: 0, Time: 1.8s"
    line="$(grep -E "Total: [0-9]+, Errors: [0-9]+, Failed: [0-9]+" "$log" | tail -n 1)"
    rm -f "$log"

    if [[ -z "$line" ]]; then
        # xUnit prints no summary line when a -class / -method filter matches nothing.
        if [[ $exe_status -eq 0 ]]; then
            summary+=("${RED}✗ $project: テストが 1 件も実行されていません（-class / -method の指定を確認してください）${RESET}")
        else
            summary+=("${RED}✗ $project: テストの実行に失敗しました（終了コード $exe_status、上の出力を確認してください）${RESET}")
        fi
        overall=1
        continue
    fi

    total=$(sed -E 's/.*Total: ([0-9]+).*/\1/' <<<"$line")
    errors=$(sed -E 's/.*Errors: ([0-9]+).*/\1/' <<<"$line")
    failed=$(sed -E 's/.*Failed: ([0-9]+).*/\1/' <<<"$line")
    skipped=$(sed -E 's/.*Skipped: ([0-9]+).*/\1/' <<<"$line")

    if [[ "$failed" -gt 0 || "$errors" -gt 0 || "$total" -eq 0 || $exe_status -ne 0 ]]; then
        reason="失敗 $failed / エラー $errors"
        [[ "$total" -eq 0 ]] && reason="テストが 1 件も実行されていません"
        summary+=("${RED}✗ $project: 全 $total 件、$reason（終了コード $exe_status）${RESET}")
        overall=1
    else
        summary+=("${GREEN}✓ $project: 全 $total 件 成功${RESET}$([[ "$skipped" -gt 0 ]] && echo "（スキップ $skipped）")")
    fi
done

echo
echo "${BOLD}== 結果${RESET}"
printf '%s\n' "${summary[@]}"
exit $overall
