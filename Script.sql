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


ALTER TABLE d_company ADD SupStatementNote [nvarchar](200)

ALTER TABLE d_Payroll ADD MIDPAY money
ALTER TABLE d_TransportersPayRoll ADD MIDPAY money

ALTER TABLE USERGROUPS ADD BalanceSheet bit, Bills bit, BranchMilkEnquiry bit, Budgettings bit, CashShares bit, 
ChartsofAcc bit, CofPayroll bit, CofPricing bit, Coffee bit, CreditNotes bit, CustomerInvoices bit,
CustomerProducts bit, DedFarmer bit, DedStaff bit, DedTransport bit, DefaultDed bit, Dispatch bit,
Glinquiry bit, ImportIntake bit, IncomeStatement bit, IntakeCorrection bit, JournalListing bit, 
JournalPosting bit, Marketers bit, Marketing bit, MilkControl bit, MilkTest bit, Millers bit,
Milling bit, Payroll bit, ProdDispatch bit, ProdIntake bit, ProdSales bit, ProdSupplier bit,
Products bit, Pulping bit, RCustomer bit, RFarmers bit, RImportS bit, 
RTransporter bit, RVendor bit, Refunds bit, SalesReturn bit, SendSms bit, SetBanks bit,
SetBanksBranch bit, SetCounties bit, SetDebtors bit, SetDedTypes bit, SetFarmersDif bit, SetLocation bit,
SetOrgBranch bit, SetOrganization bit, SetPrice bit, SetProducts bit, SetRoutes bit, SetSharesCat bit,
SetSubCounties bit, SetTaxes bit, SetUserGroups bit, SetUsers bit, SetWards bit, SetZones bit,
StandingOrder bit, SupplierStatement bit, TransporterAssign bit, TransporterStatement bit, TrialBalance bit, 
VarBalancing bit, VendorProducts bit

UPDATE [dbo].[USERGROUPS]
   SET [Registration] = 1
      ,[Activity] = 1
      ,[Reports] = 1
      ,[Setup] = 1
      ,[files] = 1
      ,[Accounts] = 1
      ,[Deductions] = 1
      ,[SaccoReports] = 1
      ,[Staff] = 1
      ,[Store] = 1
      ,[Flmd] = 1
      ,[BalanceSheet] = 1
      ,[Bills] = 1
      ,[BranchMilkEnquiry] = 1
      ,[Budgettings] = 1
      ,[CashShares] = 1
      ,[ChartsofAcc] = 1
      ,[CofPayroll] = 1
      ,[CofPricing] = 1
      ,[Coffee] = 1
      ,[CreditNotes] = 1
      ,[CustomerInvoices] = 1
      ,[CustomerProducts] = 1
      ,[DedFarmer] = 1
      ,[DedStaff] = 1
      ,[DedTransport] = 1
      ,[DefaultDed] = 1
      ,[Dispatch] = 1
      ,[Glinquiry] = 1
      ,[ImportIntake] = 1
      ,[IncomeStatement] = 1
      ,[IntakeCorrection] = 1
      ,[JournalListing] = 1
      ,[JournalPosting] = 1
      ,[Marketers] = 1
      ,[Marketing] = 1
      ,[MilkControl] = 1
      ,[MilkTest] = 1
      ,[Millers] = 1
      ,[Milling] = 1
      ,[Payroll] = 1
      ,[ProdDispatch] = 1
      ,[ProdIntake] = 1
      ,[ProdSales] = 1
      ,[ProdSupplier] = 1
      ,[Products] = 1
      ,[Pulping] = 1
      ,[RCustomer] = 1
      ,[RFarmers] = 1
      ,[RImportS] = 1
      ,[RTransporter] = 1
      ,[RVendor] = 1
      ,[Refunds] = 1
      ,[SalesReturn] = 1
      ,[SendSms] = 1
      ,[SetBanks] = 1
      ,[SetBanksBranch] = 1
      ,[SetCounties] = 1
      ,[SetDebtors] = 1
      ,[SetDedTypes] = 1
      ,[SetFarmersDif] = 1
      ,[SetLocation] = 1
      ,[SetOrgBranch] = 1
      ,[SetOrganization] = 1
      ,[SetPrice] = 1
      ,[SetProducts] = 1
      ,[SetRoutes] = 1
      ,[SetSharesCat] = 1
      ,[SetSubCounties] = 1
      ,[SetTaxes] = 1
      ,[SetUserGroups] = 1
      ,[SetUsers] = 1
      ,[SetWards] = 1
      ,[SetZones] = 1
      ,[StandingOrder] = 1
      ,[SupplierStatement] = 1
      ,[TransporterAssign] = 1
      ,[TransporterStatement] = 1
      ,[TrialBalance] = 1
      ,[VarBalancing] = 1
      ,[VendorProducts] = 1
GO




/****** Object:  Table [dbo].[d_DCodesQuantity]    Script Date: 30-Jul-23 12:54:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[d_DCodesQuantity](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Month] [nvarchar](50) NULL,
	[TotalSales] [money] NULL,
 CONSTRAINT [PK_d_DCodesQuantity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
