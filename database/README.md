# データベース設計

## 概要
SQL Serverを使用した事務用品管理システムのデータベース設計です。

## テーブル構成

### 1. Departments（部署マスタ）
| カラム名 | 型 | 説明 |
|---------|-----|------|
| DepartmentId | INT | 主キー |
| DepartmentCode | NVARCHAR(20) | 部署コード |
| DepartmentName | NVARCHAR(100) | 部署名 |
| IsActive | BIT | 有効フラグ |

### 2. Items（物品マスタ）
| カラム名 | 型 | 説明 |
|---------|-----|------|
| ItemId | INT | 主キー |
| ItemCode | NVARCHAR(50) | バーコード/QRコード |
| ItemName | NVARCHAR(200) | 物品名 |
| Unit | NVARCHAR(20) | 単位 |
| CurrentStock | INT | 現在庫数 |
| MinimumStock | INT | 最小在庫数 |

### 3. Transactions（取引履歴）
| カラム名 | 型 | 説明 |
|---------|-----|------|
| TransactionId | INT | 主キー |
| ItemId | INT | 物品ID（外部キー） |
| DepartmentId | INT | 部署ID（外部キー） |
| TransactionType | NVARCHAR(10) | IN/OUT |
| Quantity | INT | 数量 |
| BeforeStock | INT | 処理前在庫 |
| AfterStock | INT | 処理後在庫 |

## セットアップ手順
1. SQL Serverに接続
2. `schema/001_create_database.sql`を実行
3. `schema/002_create_tables.sql`を実行
4. `seed/01_initial_data.sql`を実行（テストデータ）