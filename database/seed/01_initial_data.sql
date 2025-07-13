-- 初期データ投入スクリプト
USE OfficeSuppliesDB;
GO

-- 部署マスタ初期データ
INSERT INTO Departments (DepartmentCode, DepartmentName) VALUES
    (N'HR', N'人事部'),
    (N'GA', N'総務部'),
    (N'ACC', N'経理部'),
    (N'SALES', N'営業部'),
    (N'IT', N'情報システム部'),
    (N'DEV', N'開発部');
GO

-- 物品マスタ初期データ（テスト用）
INSERT INTO Items (ItemCode, ItemName, ItemDescription, Unit, CurrentStock, MinimumStock) VALUES
    (N'4901681181814', N'ボールペン（黒）', N'三菱鉛筆 ジェットストリーム 0.7mm', N'本', 100, 20),
    (N'4901681181821', N'ボールペン（赤）', N'三菱鉛筆 ジェットストリーム 0.7mm', N'本', 50, 10),
    (N'4901681181838', N'ボールペン（青）', N'三菱鉛筆 ジェットストリーム 0.7mm', N'本', 50, 10),
    (N'4902505544194', N'A4コピー用紙', N'富士ゼロックス 500枚入り', N'冊', 30, 10),
    (N'4901480005546', N'付箋（75×75mm）', N'ポスト・イット 黄色', N'個', 200, 50),
    (N'4970051025634', N'ホッチキス針', N'MAX No.10-1M', N'箱', 50, 20),
    (N'4902870726256', N'クリアファイル', N'コクヨ A4 透明', N'枚', 500, 100),
    (N'4901331502866', N'消しゴム', N'トンボ MONO', N'個', 100, 30);
GO

-- テスト用取引データ
INSERT INTO Transactions (ItemId, DepartmentId, TransactionType, Quantity, BeforeStock, AfterStock, ProcessedBy, Remarks) VALUES
    (1, 1, 'OUT', 5, 100, 95, N'システム管理者', N'初期テストデータ'),
    (4, 2, 'OUT', 2, 30, 28, N'システム管理者', N'初期テストデータ'),
    (1, 1, 'IN', 50, 95, 145, N'システム管理者', N'在庫補充');
GO