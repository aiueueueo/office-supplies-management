-- テーブル作成スクリプト
USE OfficeSuppliesDB;
GO

-- 部署マスタ
CREATE TABLE Departments (
    DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentCode NVARCHAR(20) NOT NULL UNIQUE,
    DepartmentName NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- 物品マスタ
CREATE TABLE Items (
    ItemId INT IDENTITY(1,1) PRIMARY KEY,
    ItemCode NVARCHAR(50) NOT NULL UNIQUE,  -- バーコード/QRコード
    ItemName NVARCHAR(200) NOT NULL,
    ItemDescription NVARCHAR(500),
    Unit NVARCHAR(20) NOT NULL DEFAULT N'個',  -- 単位（個、本、箱など）
    CurrentStock INT NOT NULL DEFAULT 0,
    MinimumStock INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- 取引履歴
CREATE TABLE Transactions (
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    ItemId INT NOT NULL,
    DepartmentId INT NOT NULL,
    TransactionType NVARCHAR(10) NOT NULL,  -- 'IN'（入庫）, 'OUT'（出庫）
    Quantity INT NOT NULL,
    BeforeStock INT NOT NULL,
    AfterStock INT NOT NULL,
    Remarks NVARCHAR(500),
    ProcessedBy NVARCHAR(100) NOT NULL,  -- 処理実行者
    ProcessedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsCancelled BIT NOT NULL DEFAULT 0,
    CancelledAt DATETIME2,
    CancelledBy NVARCHAR(100),
    CONSTRAINT FK_Transactions_Items FOREIGN KEY (ItemId) REFERENCES Items(ItemId),
    CONSTRAINT FK_Transactions_Departments FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId),
    CONSTRAINT CK_TransactionType CHECK (TransactionType IN ('IN', 'OUT'))
);
GO

-- インデックス作成
CREATE INDEX IX_Transactions_ItemId ON Transactions(ItemId);
CREATE INDEX IX_Transactions_DepartmentId ON Transactions(DepartmentId);
CREATE INDEX IX_Transactions_ProcessedAt ON Transactions(ProcessedAt DESC);
CREATE INDEX IX_Items_ItemCode ON Items(ItemCode);
GO