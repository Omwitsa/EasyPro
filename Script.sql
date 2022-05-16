CREATE TABLE ProductIntake (
	Id bigint IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	Sno bigint NOT NULL,
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
	Branch nvarchar(50)
);
