#!/bin/bash
# GitHub Issues確認スクリプト
# 使用前提: GitHub CLI (gh) がインストール済み

REPO="aiueueueo/office-supplies-management"

echo "=== GitHub Issues 一覧 ==="
gh issue list --repo $REPO --limit 10

echo -e "\n=== 最新のIssue詳細 ==="
LATEST_ISSUE=$(gh issue list --repo $REPO --limit 1 --json number --jq '.[0].number')
if [ ! -z "$LATEST_ISSUE" ]; then
    gh issue view $LATEST_ISSUE --repo $REPO
fi