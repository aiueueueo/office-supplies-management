-- テーブル作成スクリプト
USE OfficeSuppliesDB;
GO

-- =============================================
-- 部署マスタテーブル
-- 役割：組織内の部署情報を管理
-- =============================================
CREATE TABLE Departments (
    -- 部署の一意識別子（自動採番）
    DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
    
    -- 部署コード（業務で使用する略称。例：HR、GA、SALES）
    -- 重複不可、システム内で部署を特定する際に使用
    DepartmentCode NVARCHAR(20) NOT NULL UNIQUE,
    
    -- 部署の正式名称（例：人事部、総務部、営業部）
    DepartmentName NVARCHAR(100) NOT NULL,
    
    -- 有効フラグ（1：有効、0：無効）
    -- 部署の廃止時は物理削除せず、このフラグで論理削除
    IsActive BIT NOT NULL DEFAULT 1,
    
    -- レコード作成日時（監査用）
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- レコード最終更新日時（監査用）
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- 物品マスタテーブル
-- 役割：管理対象の事務用品情報を保持
-- =============================================
CREATE TABLE Items (
    -- 物品の一意識別子（自動採番）
    ItemId INT IDENTITY(1,1) PRIMARY KEY,
    
    -- バーコード/QRコード値
    -- スキャナーで読み取った値をそのまま格納
    -- 重複不可、物品検索のキーとなる
    ItemCode NVARCHAR(50) NOT NULL UNIQUE,
    
    -- 物品名（例：ボールペン（黒）、A4コピー用紙）
    ItemName NVARCHAR(200) NOT NULL,
    
    -- 物品の詳細説明（メーカー名、型番など補足情報）
    -- NULL許可：詳細情報が不要な物品もある
    ItemDescription NVARCHAR(500),
    
    -- 数量の単位（例：個、本、箱、冊、枚）
    -- デフォルトは「個」
    Unit NVARCHAR(20) NOT NULL DEFAULT N'個',
    
    -- 現在の在庫数
    -- 出入庫処理で自動更新される
    CurrentStock INT NOT NULL DEFAULT 0,
    
    -- 最小在庫数（この数を下回ると発注検討）
    -- 在庫管理の警告基準値
    MinimumStock INT NOT NULL DEFAULT 0,
    
    -- 有効フラグ（1：有効、0：無効）
    -- 廃番商品は論理削除で対応
    IsActive BIT NOT NULL DEFAULT 1,
    
    -- レコード作成日時（監査用）
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- レコード最終更新日時（監査用）
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- 取引履歴テーブル
-- 役割：全ての出入庫の履歴を記録（監査証跡）
-- =============================================
CREATE TABLE Transactions (
    -- 取引の一意識別子（自動採番）
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    
    -- 対象物品のID（Itemsテーブルへの外部キー）
    ItemId INT NOT NULL,
    
    -- 取引を行った部署のID（Departmentsテーブルへの外部キー）
    DepartmentId INT NOT NULL,
    
    -- 取引種別
    -- 'IN'：入庫（在庫追加）、'OUT'：出庫（在庫減少）
    -- CHECK制約で値を制限
    TransactionType NVARCHAR(10) NOT NULL,
    
    -- 取引数量（必ず正の値）
    Quantity INT NOT NULL,
    
    -- 処理前の在庫数（履歴追跡用）
    BeforeStock INT NOT NULL,
    
    -- 処理後の在庫数（履歴追跡用）
    AfterStock INT NOT NULL,
    
    -- 備考欄（取引の理由や補足情報）
    -- NULL許可：必須ではない
    Remarks NVARCHAR(500),
    
    -- 処理実行者名（誰が操作したかの記録）
    ProcessedBy NVARCHAR(100) NOT NULL,
    
    -- 処理実行日時
    ProcessedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- 取消フラグ（0：有効、1：取消済み）
    -- 誤操作時の取消処理で使用
    IsCancelled BIT NOT NULL DEFAULT 0,
    
    -- 取消処理日時（取消時のみ値が入る）
    CancelledAt DATETIME2,
    
    -- 取消処理実行者（取消時のみ値が入る）
    CancelledBy NVARCHAR(100),
    
    -- 外部キー制約：物品マスタとの関連
    CONSTRAINT FK_Transactions_Items FOREIGN KEY (ItemId) REFERENCES Items(ItemId),
    
    -- 外部キー制約：部署マスタとの関連
    CONSTRAINT FK_Transactions_Departments FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId),
    
    -- チェック制約：取引種別は'IN'または'OUT'のみ
    CONSTRAINT CK_TransactionType CHECK (TransactionType IN ('IN', 'OUT'))
);
GO

-- =============================================
-- インデックス作成
-- 役割：検索性能の向上
-- =============================================

-- 物品IDでの取引検索を高速化（特定物品の履歴表示用）
CREATE INDEX IX_Transactions_ItemId ON Transactions(ItemId);

-- 部署IDでの取引検索を高速化（部署別集計用）
CREATE INDEX IX_Transactions_DepartmentId ON Transactions(DepartmentId);

-- 処理日時での取引検索を高速化（降順で最新から表示）
CREATE INDEX IX_Transactions_ProcessedAt ON Transactions(ProcessedAt DESC);

-- バーコード検索を高速化（スキャン時の物品特定用）
CREATE INDEX IX_Items_ItemCode ON Items(ItemCode);
GO