# 事務用品管理システム TODO

## 必須作業（システム動作に必要）

### 1. API側の追加エンドポイント実装
- [ ] **DepartmentsController作成**
  - `GET /api/departments` - 部署一覧取得
  - `POST /api/departments` - 部署新規作成
  - `PUT /api/departments/{id}` - 部署更新
  - `DELETE /api/departments/{id}` - 部署削除

- [ ] **TransactionsController作成**
  - `POST /api/transactions/stock-out` - 出庫処理実行
  - `POST /api/transactions/cancel-last` - 直前取引の取消
  - `GET /api/transactions` - 取引履歴取得（検索条件付き）

### 2. データベースセットアップ
- [ ] **SQL Serverインスタンス準備**
  - SQL Server Expressインストール（まだの場合）
  - データベース接続確認

- [ ] **スキーマ作成**
  - `database/schema/001_create_database.sql` 実行
  - `database/schema/002_create_tables.sql` 実行

- [ ] **初期データ投入**
  - `database/seed/01_initial_data.sql` 実行
  - テストデータ確認

### 3. Entity Framework マイグレーション
- [ ] **初回マイグレーション作成**
  ```bash
  cd backend/OfficeSupplies.Api
  dotnet ef migrations add InitialCreate
  ```

- [ ] **データベース更新**
  ```bash
  dotnet ef database update
  ```

## 修正・改善作業

### 4. Blazor管理画面の修正
- [ ] **History.razor修正**
  - IJSRuntime注入追加
  - CSV出力機能のJavaScript修正

- [ ] **StockIn.razor修正**
  - 管理部署IDのハードコード修正（部署選択機能追加）

### 5. Flutter アプリの実装改善
- [ ] **実際のバーコードスキャン実装**
  - qr_code_scannerパッケージの実装
  - カメラ権限処理
  - テスト用ダミーコードの削除

- [ ] **エラーハンドリング強化**
  - ネットワークエラー時の再試行機能
  - オフライン対応（基本機能）

- [ ] **ユーザー認証・識別**
  - 処理者名の自動入力機能
  - 部署の記憶機能

## テスト・検証

### 6. 動作テスト
- [ ] **API動作確認**
  - Swagger UIでの各エンドポイントテスト
  - データベース操作確認

- [ ] **Blazor管理画面テスト**
  - 全画面の動作確認
  - QRコード生成・表示確認
  - CSV出力機能確認

- [ ] **Flutter アプリテスト**
  - Android端末での動作確認
  - API連携テスト
  - エラーケースのテスト

### 7. 統合テスト
- [ ] **エンドツーエンドテスト**
  - Blazorで物品登録 → QRコード生成
  - FlutterでQRコードスキャン → 出庫処理
  - Blazorで履歴確認

## ドキュメント改善作業

### 11. README.md改善（PR #2レビューフィードバック）
- [ ] **セットアップ手順の具体化**
  - SQL Server接続文字列の設定方法を追加
  - `appsettings.json`の設定例を含める
  - `dotnet ef database update`等のマイグレーション適用手順を明記

- [ ] **開発状況の詳細化**
  - API実装の進捗率を具体的に表示（例：「🔄 API実装中 (60%)」）
  - 各コンポーネントの動作可能レベルを明記

- [ ] **トラブルシューティング追加**
  - よくあるセットアップエラーとその解決方法を追加
  - 起動ログとURL確認の注意事項追加

- [ ] **表示形式統一**
  - 開発状況をマークダウンチェックボックス形式に統一
  - 進捗管理の改善（クリックで完了状態更新可能）

## デプロイ・運用準備

### 8. 設定ファイル調整
- [ ] **本番環境用設定**
  - 接続文字列の環境変数化
  - ログレベル調整
  - セキュリティ設定強化

- [ ] **Flutter本番ビルド**
  - リリースモードでのAPKビルド
  - 署名設定
  - ProGuard設定

### 9. ドキュメント整備
- [ ] **運用マニュアル作成**
  - 管理者向け操作マニュアル
  - エンドユーザー向けアプリ使用説明

- [ ] **技術ドキュメント更新**
  - API仕様書
  - データベース設計書
  - 環境構築手順書

### 🔧 Slack連携デバッグ（緊急）
- [ ] **Claude Code Actions実行ログ解析**
  - GitHub Actions → Claude PR Review with Slack ワークフローの詳細ログ確認
  - MCP Slack サーバーの接続状況チェック
  - Docker実行エラーの有無確認
  - 環境変数（SLACK_BOT_TOKEN, SLACK_TEAM_ID, SLACK_CHANNEL_ID）の値検証

- [ ] **Slack Bot設定再確認**
  - Bot Token Scopesの権限確認（chat:write, channels:read等）
  - ワークスペースへのBot追加状況確認
  - 対象チャンネルへのBot招待状況確認

- [ ] **MCPサーバー設定見直し**
  - Docker イメージ mcp/slack の存在確認
  - allowed_tools設定の妥当性確認
  - MCPサーバー起動ログの確認

## 将来的な機能拡張（優先度低）

### 10. 追加機能
- [ ] **在庫アラート機能**
  - 最小在庫を下回った場合の通知
  - メール通知機能

- [ ] **レポート機能強化**
  - 月次・年次集計レポート
  - グラフ表示機能

- [ ] **承認ワークフロー**
  - 大量出庫時の承認機能
  - 部署管理者による承認

- [ ] **複数拠点対応**
  - 拠点別在庫管理
  - 拠点間移動機能

---

## 進捗管理

- 🔥 **高優先度**: 1-3（システム動作に必須）
- ⚡ **中優先度**: 4-7（品質・使いやすさ向上）
- 💡 **低優先度**: 8-10（将来的な改善）

完了したタスクは `- [x]` に変更してください。