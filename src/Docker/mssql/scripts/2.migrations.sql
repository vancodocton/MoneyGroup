USE [MoneyGroup]
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Total] decimal(18,2) NOT NULL,
    [BuyerId] int NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Users_BuyerId] FOREIGN KEY ([BuyerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [OrderParticipants] (
    [ParticipantId] int NOT NULL,
    [OrderId] int NOT NULL,
    CONSTRAINT [PK_OrderParticipants] PRIMARY KEY ([OrderId], [ParticipantId]),
    CONSTRAINT [FK_OrderParticipants_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderParticipants_Users_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_OrderParticipants_ParticipantId] ON [OrderParticipants] ([ParticipantId]);

CREATE INDEX [IX_Orders_BuyerId] ON [Orders] ([BuyerId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250303190831_AddOrderAggregate', N'9.0.3');

COMMIT;
GO

