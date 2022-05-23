USE [MORINGA]
GO

CREATE TABLE County (
	Id bigint IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [varchar](50) NULL,
	[Contact] [varchar](50) NULL,
	[Closed] [bit] NOT NULL,
	[CreatedOn] [smalldatetime] NULL,
	[CreatedBy] [varchar](50) NULL,
);

CREATE TABLE SubCounty (
	Id bigint IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [varchar](50) NULL,
	[County] [varchar](50) NULL,
	[Contact] [varchar](50) NULL,
	[Closed] [bit] NOT NULL,
	[CreatedOn] [smalldatetime] NULL,
	[CreatedBy] [varchar](50) NULL,
);

CREATE TABLE Ward (
	Id bigint IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [varchar](50) NULL,
	[SubCounty] [varchar](50) NULL,
	[Contact] [varchar](50) NULL,
	[Closed] [bit] NOT NULL,
	[CreatedOn] [smalldatetime] NULL,
	[CreatedBy] [varchar](50) NULL,
);

CREATE TABLE ProductIntake (
	Id bigint IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	Sno nvarchar(50) NOT NULL,
	TransDate datetime,
	TransTime time(7),
	ProductType nvarchar(50),
	QSupplied money,
	PPU money,
	CR money,
	DR money,
	Balance money,
	Description nvarchar(50),
	Paid bit NOT NULL,
	Remarks nvarchar(50),
	TransactionType int,
	AuditId nvarchar(50),
	auditdatetime datetime,
	Branch nvarchar(50),
	SaccoCode nvarchar(50)
);
