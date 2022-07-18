USE [MORINGA]
GO

CREATE TABLE FLMD (
	Id bigint IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Sno] [varchar](50) NULL,
	[ExoticCattle] [int] NULL,
	[IndigenousCattle] [int] NULL,
	[IndigenousChicken] [int] NULL,
	[Sheep] [int] NULL,
	[Goats] [int] NULL,
	[Camels] [int] NULL,
	[Donkeys] [int] NULL,
	[Pigs] [int] NULL,
	[BeeHives] [int] NULL,
	[Boys] [int] NULL,
	[Girls] [int] NULL,
	[Deaths] [int] NULL,
	[Disabled] [int] NULL,
	[PostGraduates] [int] NULL,
	[Graduates] [int] NULL,
	[UnderGraduates] [int] NULL,
	[Secondary] [int] NULL,
	[Primary] [int] NULL,
	[Nursery] [int] NULL,
	[SaccoCode] [varchar](50) NULL,
);

CREATE TABLE FLMDCrops (
	Id bigint IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Sno] [varchar](50) NULL,
	[CashCrops] [varchar](50) NULL,
	[ConsumerCrops] [varchar](50) NULL,
	[Vegetables] [varchar](50) NULL,
	[AnimalFeeds] [varchar](50) NULL,
	[SaccoCode] [varchar](50) NULL,
);

CREATE TABLE FLMDLand (
	Id bigint IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Sno] [varchar](50) NULL,
	[Location] [varchar](50) NULL,
	[PlotNumber] [varchar](50) NULL,
	[TotalAcres] [float] NULL,
	[AcresCrops] [float] NULL,
	[AcresBuilding] [float] NULL,
	[AcresLivestock] [float] NULL,
	[acresUnusedLand] [float] NULL,
	[SaccoCode] [varchar](50) NULL,
);

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
