# 事務用品管理システム

## 概要
事務用品の在庫管理を行うシステムです。バーコード/QRコードを使用した出庫処理と、Web画面での在庫管理機能を提供します。

## システム構成
- **backend/**: ASP.NET Core Web API + Blazor Server（管理画面）
- **mobile/**: Flutter（Android端末用アプリ）
- **database/**: SQL Serverスキーマ・スクリプト
- **docs/**: ドキュメント

## 技術スタック
- Backend: ASP.NET Core 8.0, Blazor Server, Entity Framework Core
- Mobile: Flutter
- Database: SQL Server
- バーコード: ZXing (Flutter), QRCoder (Blazor)

## 主な機能
### モバイルアプリ（出庫端末）
- バーコード/QRコード読み取り
- 部署選択
- 出庫数量入力
- 取り消し機能

### Web管理画面
- 物品マスタ管理
- QRコード生成・印刷
- 入庫処理
- 在庫一覧
- 出入庫履歴

## 開発状況
- ✅ プロジェクト基本構成完了
- ✅ データベース設計完了
- ✅ Blazor管理画面実装完了
- ✅ Flutter出庫端末実装完了
- ✅ Claude Code Actions設定完了
- 🔄 API実装中（TODO.md参照）

## セットアップ
詳細は `docs/TODO.md` を参照してください。

### 必要な環境
- .NET 8.0 SDK
- SQL Server
- Flutter SDK（モバイルアプリ開発時）

### クイックスタート
1. リポジトリをクローン
2. データベースセットアップ（`database/schema/`のSQLを実行）
3. バックエンド起動：`cd backend && dotnet run --project OfficeSupplies.Web`
4. 管理画面アクセス：`https://localhost:7001`