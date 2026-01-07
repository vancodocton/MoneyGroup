USE [master]
GO

IF DB_ID('MoneyGroup') IS NULL
CREATE DATABASE [MoneyGroup]
    COLLATE SQL_Latin1_General_CP1_CI_AS
GO
