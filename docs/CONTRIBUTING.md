# 事務用品管理システム - 開発ガイド

## 🚀 開発への参加方法

### 環境準備
1. .NET 8.0 SDK のインストール
2. SQL Server のセットアップ
3. Flutter SDK のインストール（モバイル開発時）

### 開発フロー
1. Issue を作成または既存の Issue を確認
2. ブランチを作成（例：`feature/add-api-controller`）
3. 実装・テスト
4. PR を作成
5. レビュー → マージ

## 📋 コーディング規約

### 設計原則
- **DRY (Don't Repeat Yourself)**: コードの重複を避ける
- **YAGNI (You Aren't Gonna Need It)**: 不要な機能は実装しない
- **KISS (Keep It Simple, Stupid)**: シンプルに保つ

### C# (.NET)
- PascalCase: クラス名、メソッド名
- camelCase: フィールド名、ローカル変数
- 非同期メソッドには `Async` サフィックス
- Entity Framework規約に従う

### Flutter (Dart)
- snake_case: ファイル名、変数名
- PascalCase: クラス名
- lowerCamelCase: メソッド名
- 状態管理は Provider パターンを推奨

## 🧪 テスト

### バックエンド
```bash
cd backend
dotnet test
```

### フロントエンド
```bash
cd mobile/office_supplies_mobile
flutter test
```

## 📊 プロジェクト構成

### ディレクトリ構造
```
├── backend/           # ASP.NET Core
│   ├── Api/          # Web API
│   ├── Core/         # エンティティ
│   ├── Infrastructure/ # データベース
│   └── Web/          # Blazor管理画面
├── mobile/           # Flutter
├── database/         # SQL スクリプト
└── docs/            # ドキュメント
```

## 🤖 Claude Code Actions

### 自動化機能
- PRレビュー（コード品質チェック）
- バグ調査支援
- 実装支援
- Slack通知

### 使い方
```markdown
@claude この機能の実装をお願いします

要件：
- 〇〇機能の追加
- 〇〇のバグ修正
```

## 🎯 優先タスク

現在の優先順位は `docs/TODO.md` を参照してください。

### 貢献しやすいタスク
1. APIコントローラーの実装
2. テストコードの追加
3. ドキュメントの改善
4. バグ修正

## 📞 質問・相談

- Issue でのディスカッション
- PR レビューでのコメント
- `@claude` メンションで自動サポート

---

**みなさんの貢献をお待ちしています！** 🎉