#!/bin/bash
# 予期せぬエラー時に即座に終了させる
set -euo pipefail

# 必要なコマンドがインストールされているか確認
if ! command -v gh >/dev/null 2>&1; then
  echo "Error: GitHub CLI 'gh' is not installed." >&2
  echo "Please install GitHub CLI: https://cli.github.com/" >&2
  exit 1
fi

if ! command -v jq >/dev/null 2>&1; then
  echo "Error: 'jq' is not installed." >&2
  echo "Please install jq: https://stedolan.github.io/jq/" >&2
  exit 1
fi

# GitHub Issues確認スクリプト
# 使用前提: GitHub CLI (gh) がインストール済み

REPO="aiueueueo/office-supplies-management"

echo "=== GitHub Issues 一覧 ==="
gh issue list --repo "${REPO}" --limit 10

echo -e "\n=== 最新のIssue詳細 ==="
# 最新 Issue 番号を取得
LATEST_ISSUE=$(gh issue list --repo "${REPO}" --limit 1 --json number --jq '.[0].number' || echo "")

if [[ -n $LATEST_ISSUE ]]; then
    gh issue view "$LATEST_ISSUE" --repo "$REPO"
else
    echo "最新の Issue は存在しません。"
fi