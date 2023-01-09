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

add column extension to dpayroll and dtransporterspayroll table
add column Flmd to USERGROUPS
add column SMS to dpayroll and dtransporterspayroll table
change startdate type to datetime in d_PreSets

