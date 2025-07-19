# 事務用品管理システム - Claude設定

## プロジェクト概要
事務用品の在庫管理を行うシステムです。バーコード/QRコードを使用した出庫処理と、Web画面での在庫管理機能を提供します。

## システム構成
- **backend/**: ASP.NET Core Web API + Blazor Server（管理画面）+ .NET MAUI（出庫端末アプリ）
- **mobile/**: Flutter（Android端末用アプリ - 廃止予定）
- **database/**: SQL Serverスキーマ・スクリプト
- **docs/**: ドキュメント

## 技術スタック
- Backend: ASP.NET Core 8.0, Blazor Server, Entity Framework Core
- Mobile: Flutter（廃止予定）, .NET MAUI（Android/Windows対応）
- Database: SQL Server
- バーコード: ZXing (Flutter), ZXing.Net.Maui (MAUI), QRCoder (Blazor)

## Claude Code設定

### 言語設定
Claude CodeまたはClaude Code Actionsでは、このプロジェクトに関する全ての応答を**日本語**で行ってください。

### 開発方針
- C#のコーディング規約に従う
- Flutterのベストプラクティスに従う
- セキュリティを重視した実装
- ユーザビリティを考慮したUI/UX設計

### 設計原則（必須遵守）
#### DRY (Don't Repeat Yourself) - 重複排除
- 同じロジックやコードの重複を避ける
- 共通機能は基底クラスやサービスクラスに抽出
- 設定値は一箇所で管理（appsettings.json、環境変数等）
- データベースアクセスパターンの共通化

#### YAGNI (You Aren't Gonna Need It) - 不要な機能は実装しない
- 現在必要な機能のみを実装
- 将来の拡張性を考慮しつつも、過度な抽象化は避ける
- 推測に基づく機能追加は行わない
- シンプルで理解しやすいコード優先

#### KISS (Keep It Simple, Stupid) - シンプルに保つ
- 複雑な設計パターンは必要な場合のみ使用
- 読みやすく保守しやすいコードを心がける
- 過度な最適化は避ける
- 明確で分かりやすい命名規則

### 主要機能
#### モバイルアプリ（出庫端末）
- バーコード/QRコード読み取り
- 部署選択
- 出庫数量入力
- 取り消し機能

#### Web管理画面
- 物品マスタ管理
- QRコード生成・印刷
- 入庫処理
- 在庫一覧
- 出入庫履歴

### ファイル構成
```
office-supplies-management/
├── backend/
│   ├── OfficeSupplies.Api/          # Web API
│   ├── OfficeSupplies.Core/         # エンティティ
│   ├── OfficeSupplies.Infrastructure/ # データベース
│   ├── OfficeSupplies.Web/          # Blazor管理画面
│   └── OfficeSupplies.Mobile.MAUI/  # MAUI出庫端末（Android/Windows）
├── mobile/
│   └── office_supplies_mobile/      # Flutter出庫端末（廃止予定）
├── database/
│   ├── schema/                      # テーブル定義
│   └── seed/                        # 初期データ
└── docs/
    ├── TODO.md                      # 残作業リスト
    └── CLAUDE.md                    # この設定ファイル
```

### 残作業
詳細は `docs/TODO.md` を参照してください。

### 注意事項
- データベース接続文字列は環境に応じて調整が必要
- Flutterアプリのカメラスキャン機能は現在テスト用ダミー実装
- MAUIアプリのバーコードスキャン機能は開発・テスト環境でモック実装を使用（Android環境下のみ）
- 本番運用前にセキュリティ設定の見直しが必要