USE [MoneyGroup]
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([Id], [Name]) VALUES (1, N'Truong')
GO
INSERT [dbo].[Users] ([Id], [Name]) VALUES (2, N'Duc')
GO
INSERT [dbo].[Users] ([Id], [Name]) VALUES (3, N'Manh')
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 
GO
INSERT [dbo].[Orders] ([Id], [Title], [Description], [Total], [BuyerId]) VALUES (1, N'Order 1', N'Order 1 description', CAST(10000.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Orders] ([Id], [Title], [Description], [Total], [BuyerId]) VALUES (2, N'Update order', N'Update order description', CAST(10000.00 AS Decimal(18, 2)), 1)
GO
INSERT [dbo].[Orders] ([Id], [Title], [Description], [Total], [BuyerId]) VALUES (3, N'Delete order', N'Delete order description', CAST(10000.00 AS Decimal(18, 2)), 1)
GO
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (1, 1)
GO
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (1, 2)
GO
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (1, 3)
GO
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (2, 1)
GO
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (2, 2)
GO
INSERT [dbo].[OrderParticipants] ([ParticipantId], [OrderId]) VALUES (2, 3)
GO
