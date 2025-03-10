USE [master]
GO
/****** Object:  Database [MoneyGroup]    Script Date: 3/4/2025 1:32:59 AM ******/
CREATE DATABASE [MoneyGroup]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MoneyGroup', FILENAME = N'/var/opt/mssql/data/MoneyGroup.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'MoneyGroup_log', FILENAME = N'/var/opt/mssql/data/MoneyGroup_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 COLLATE SQL_Latin1_General_CP1_CI_AS
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [MoneyGroup] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MoneyGroup].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MoneyGroup] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MoneyGroup] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MoneyGroup] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MoneyGroup] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MoneyGroup] SET ARITHABORT OFF 
GO
ALTER DATABASE [MoneyGroup] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MoneyGroup] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MoneyGroup] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MoneyGroup] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MoneyGroup] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MoneyGroup] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MoneyGroup] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MoneyGroup] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MoneyGroup] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MoneyGroup] SET  ENABLE_BROKER 
GO
ALTER DATABASE [MoneyGroup] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MoneyGroup] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MoneyGroup] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MoneyGroup] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MoneyGroup] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MoneyGroup] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [MoneyGroup] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MoneyGroup] SET RECOVERY FULL 
GO
ALTER DATABASE [MoneyGroup] SET  MULTI_USER 
GO
ALTER DATABASE [MoneyGroup] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MoneyGroup] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MoneyGroup] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MoneyGroup] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MoneyGroup] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [MoneyGroup] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'MoneyGroup', N'ON'
GO
ALTER DATABASE [MoneyGroup] SET QUERY_STORE = ON
GO
ALTER DATABASE [MoneyGroup] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [MoneyGroup]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 3/4/2025 1:32:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderParticipants]    Script Date: 3/4/2025 1:32:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderParticipants](
	[ParticipantId] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
 CONSTRAINT [PK_OrderParticipants] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC,
	[ParticipantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 3/4/2025 1:32:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[BuyerId] [int] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 3/4/2025 1:32:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250303190831_AddOrderAggregate', N'9.0.2')
GO
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (1, 1)
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (1, 2)
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (1, 3)
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (2, 1)
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (2, 2)
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (2, 3)
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([Id], [Title], [Description], [Total], [BuyerId]) VALUES (1, N'Order 1', N'Order 1 description', CAST(10000.00 AS Decimal(18, 2)), 1)
INSERT [dbo].[Orders] ([Id], [Title], [Description], [Total], [BuyerId]) VALUES (2, N'Update order', N'Update order description', CAST(10000.00 AS Decimal(18, 2)), 1)
INSERT [dbo].[Orders] ([Id], [Title], [Description], [Total], [BuyerId]) VALUES (3, N'Delete order', N'Delete order description', CAST(10000.00 AS Decimal(18, 2)), 1)
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([Id], [Name]) VALUES (1, N'Truong')
INSERT [dbo].[Users] ([Id], [Name]) VALUES (2, N'Duc')
INSERT [dbo].[Users] ([Id], [Name]) VALUES (3, N'Manh')
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
/****** Object:  Index [IX_OrderParticipants_ParticipantId]    Script Date: 3/4/2025 1:32:59 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderParticipants_ParticipantId] ON [dbo].[OrderParticipants]
(
	[ParticipantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_BuyerId]    Script Date: 3/4/2025 1:32:59 AM ******/
CREATE NONCLUSTERED INDEX [IX_Orders_BuyerId] ON [dbo].[Orders]
(
	[BuyerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[OrderParticipants]  WITH CHECK ADD  CONSTRAINT [FK_OrderParticipants_Orders_OrderId] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderParticipants] CHECK CONSTRAINT [FK_OrderParticipants_Orders_OrderId]
GO
ALTER TABLE [dbo].[OrderParticipants]  WITH CHECK ADD  CONSTRAINT [FK_OrderParticipants_Users_ParticipantId] FOREIGN KEY([ParticipantId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[OrderParticipants] CHECK CONSTRAINT [FK_OrderParticipants_Users_ParticipantId]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Users_BuyerId] FOREIGN KEY([BuyerId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Users_BuyerId]
GO
USE [master]
GO
ALTER DATABASE [MoneyGroup] SET  READ_WRITE 
GO
