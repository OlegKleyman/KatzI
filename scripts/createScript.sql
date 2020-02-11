CREATE DATABASE Katz
GO
USE KATZ
CREATE TABLE [Books] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Author] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Rating] int NOT NULL,
    [Image] varbinary(max) NOT NULL,
    [ImageMimeType] nvarchar(max) NOT NULL,
    [Series] nvarchar(max) NULL,
    CONSTRAINT [PK_Books] PRIMARY KEY ([Id])
);