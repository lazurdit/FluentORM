create database TopCorp_DB
GO
USE [TopCorp_DB]
GO
/****** Object:  Table [dbo].[Tbl_Class]    Script Date: 9/12/2024 5:37:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_Class](
	[Fld_Class_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Fld_Class_Name] [nvarchar](250) NOT NULL,
	[Fld_Instructor_Id] [bigint] NULL,
 CONSTRAINT [PK_Tbl_Class] PRIMARY KEY CLUSTERED 
(
	[Fld_Class_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tbl_Instructor]    Script Date: 9/12/2024 5:37:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_Instructor](
	[Fld_Instructor_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Fld_Instructor_Name] [nvarchar](250) NOT NULL,
	[Fld_Class_Id] [bigint] NULL,
 CONSTRAINT [PK_Tbl_Instructor] PRIMARY KEY CLUSTERED 
(
	[Fld_Instructor_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tbl_Student]    Script Date: 9/12/2024 5:37:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_Student](
	[Fld_Student_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Fld_Class_Id] [bigint] NULL,
	[Fld_Student_Name] [nvarchar](255) NOT NULL,
	[Fld_Student_Created_At] [datetimeoffset](7) NOT NULL,
	[Fld_Student_Updated_At] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_Tbl_Student] PRIMARY KEY CLUSTERED 
(
	[Fld_Student_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Tbl_Class] ON 
GO
INSERT [dbo].[Tbl_Class] ([Fld_Class_Id], [Fld_Class_Name], [Fld_Instructor_Id]) VALUES (1, N'Class 1', NULL)
GO
INSERT [dbo].[Tbl_Class] ([Fld_Class_Id], [Fld_Class_Name], [Fld_Instructor_Id]) VALUES (2, N'Class 2', NULL)
GO
INSERT [dbo].[Tbl_Class] ([Fld_Class_Id], [Fld_Class_Name], [Fld_Instructor_Id]) VALUES (3, N'Class 3', NULL)
GO
INSERT [dbo].[Tbl_Class] ([Fld_Class_Id], [Fld_Class_Name], [Fld_Instructor_Id]) VALUES (4, N'Class 4', NULL)
GO
INSERT [dbo].[Tbl_Class] ([Fld_Class_Id], [Fld_Class_Name], [Fld_Instructor_Id]) VALUES (5, N'Class 5', NULL)
GO
SET IDENTITY_INSERT [dbo].[Tbl_Class] OFF
GO
SET IDENTITY_INSERT [dbo].[Tbl_Instructor] ON 
GO
INSERT [dbo].[Tbl_Instructor] ([Fld_Instructor_Id], [Fld_Instructor_Name], [Fld_Class_Id]) VALUES (1, N'Instructor 1', 5)
GO
SET IDENTITY_INSERT [dbo].[Tbl_Instructor] OFF
GO
ALTER TABLE [dbo].[Tbl_Class]  WITH CHECK ADD  CONSTRAINT [FK_Tbl_Class_Tbl_Instructor] FOREIGN KEY([Fld_Instructor_Id])
REFERENCES [dbo].[Tbl_Instructor] ([Fld_Instructor_Id])
GO
ALTER TABLE [dbo].[Tbl_Class] CHECK CONSTRAINT [FK_Tbl_Class_Tbl_Instructor]
GO
ALTER TABLE [dbo].[Tbl_Instructor]  WITH CHECK ADD  CONSTRAINT [FK_Tbl_Instructor_Tbl_Class] FOREIGN KEY([Fld_Class_Id])
REFERENCES [dbo].[Tbl_Class] ([Fld_Class_Id])
GO
ALTER TABLE [dbo].[Tbl_Instructor] CHECK CONSTRAINT [FK_Tbl_Instructor_Tbl_Class]
GO
ALTER TABLE [dbo].[Tbl_Student]  WITH NOCHECK ADD  CONSTRAINT [FK_Tbl_Student_Tbl_Class] FOREIGN KEY([Fld_Class_Id])
REFERENCES [dbo].[Tbl_Class] ([Fld_Class_Id])
GO
ALTER TABLE [dbo].[Tbl_Student] CHECK CONSTRAINT [FK_Tbl_Student_Tbl_Class]
GO
