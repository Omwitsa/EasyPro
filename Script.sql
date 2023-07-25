USE [MORINGA]
GO

CREATE TABLE [dbo].[d_SupplierCumlative](
	[Id] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[SNo] [nvarchar](50) NOT NULL,
	[Names] [nvarchar](50) NULL,
	[Cummulative] [money] NULL,
	[AuditId] [nvarchar](50) NULL,
	[Date] [datetime] NULL,
	[Branch] [nvarchar](50) NULL,
	[SaccoCode] [nvarchar](50) NULL,
	[ProductType] [nvarchar](50) NULL,
 CONSTRAINT [PK_d_SupplierCumlative] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE FLMD ADD ExoticCattleValue money, IndigenousCattleValue money, IndigenousChickenValue money, SheepValue money, GoatsValue money, CamelsValue money, DonkeysValue money, PigsValue money, BeeHivesValue money
ALTER TABLE FLMDCrops ADD CashCropsValue money, ConsumerCropsValue money, VegetablesValue money, AnimalFeedsValue money
ALTER TABLE FLMDLand ADD PlotValue money
