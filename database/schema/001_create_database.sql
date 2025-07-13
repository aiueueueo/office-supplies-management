-- 事務用品管理システム データベース作成
-- 注意: データベース名は環境に応じて変更してください

USE master;
GO

-- データベースが存在する場合は削除（開発環境のみ）
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'OfficeSuppliesDB')
BEGIN
    DROP DATABASE OfficeSuppliesDB;
END
GO

-- データベース作成
CREATE DATABASE OfficeSuppliesDB
COLLATE Japanese_CI_AS;
GO

USE OfficeSuppliesDB;
GO