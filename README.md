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

## セットアップ
（後日追記）