USE [MORINGA]
GO

CREATE TABLE [dbo].[d_PreSets](
	[Id] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[SNo] [nvarchar](50) NULL,
	[Deduction] [varchar](50) NULL,
	[Remark] [varchar](150) NULL,
	[StartDate] [varchar](50) NULL,
	[Rate] [money] NULL,
	[Stopped] [bit] NULL,
	[Auditdatetime] [datetime] NULL,
	[AuditId] [varchar](50) NULL,
	[Rated] [bit] NULL,
	[status] [bigint] NULL,
	[status2] [bigint] NULL,
	[status3] [bigint] NULL,
	[status4] [bigint] NULL,
	[status5] [bigint] NULL,
	[status6] [bigint] NULL,
	[BranchCode] [nvarchar](50) NULL,
	[saccocode] [nvarchar](50) NULL
) ON [PRIMARY]
GO


add Remarks to Dispatch table