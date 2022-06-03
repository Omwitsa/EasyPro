using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace EasyPro.Models
{
    public partial class MORINGAContext : DbContext
    {
        public MORINGAContext()
        {
        }

        public MORINGAContext(DbContextOptions<MORINGAContext> options)
        : base(options)
        {
        }

        public virtual DbSet<AgClose> AgCloses { get; set; }
        public virtual DbSet<AgClosingStock> AgClosingStocks { get; set; }
        public virtual DbSet<AgLpo> AgLpos { get; set; }
        public virtual DbSet<AgMoringaProduct> AgMoringaProducts { get; set; }
        public virtual DbSet<AgMoringaintake> AgMoringaintakes { get; set; }
        public virtual DbSet<AgOpbal> AgOpbals { get; set; }
        public virtual DbSet<AgPaging> AgPagings { get; set; }
        public virtual DbSet<AgProduct> AgProducts { get; set; }
        public virtual DbSet<AgProducts1> AgProducts1s { get; set; }
        public virtual DbSet<AgProducts3> AgProducts3s { get; set; }
        public virtual DbSet<AgProducts4> AgProducts4s { get; set; }
        public virtual DbSet<AgProducts5> AgProducts5s { get; set; }
        public virtual DbSet<AgReceipt> AgReceipts { get; set; }
        public virtual DbSet<AgReceipts1> AgReceipts1s { get; set; }
        public virtual DbSet<AgReceipts3> AgReceipts3s { get; set; }

        internal Task GetDataAsync()
        {
            throw new NotImplementedException();
        }

        public virtual DbSet<AgReceiptsEnqury> AgReceiptsEnquries { get; set; }
        public virtual DbSet<AgReceiptsProcess> AgReceiptsProcesses { get; set; }
        public virtual DbSet<AgReceiptsalesrep> AgReceiptsalesreps { get; set; }
        public virtual DbSet<AgSale> AgSales { get; set; }
        public virtual DbSet<AgStockbalance> AgStockbalances { get; set; }
        public virtual DbSet<AgStockbalance1> AgStockbalance1s { get; set; }
        public virtual DbSet<AgSupplier> AgSuppliers { get; set; }
        public virtual DbSet<AgSupplier1> AgSupplier1s { get; set; }
        public virtual DbSet<Ap> Aps { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<Assetcode> Assetcodes { get; set; }
        public virtual DbSet<AssetsRegister> AssetsRegisters { get; set; }
        public virtual DbSet<Assetstran> Assetstrans { get; set; }
        public virtual DbSet<Assetstrans1> Assetstrans1s { get; set; }
        public virtual DbSet<Assetstrans2> Assetstrans2s { get; set; }
        public virtual DbSet<Audittable> Audittables { get; set; }
        public virtual DbSet<Audittran> Audittrans { get; set; }
        public virtual DbSet<Audittrans1> Audittrans1s { get; set; }
        public virtual DbSet<B2cdisbursementResponse> B2cdisbursementResponses { get; set; }
        public virtual DbSet<B2cpaymentDummy> B2cpaymentDummies { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<BankAccount> BankAccounts { get; set; }
        public virtual DbSet<BankRecon> BankRecons { get; set; }
        public virtual DbSet<Banks1> Banks1s { get; set; }
        public virtual DbSet<Barcodedsale> Barcodedsales { get; set; }
        public virtual DbSet<Barcodeitem> Barcodeitems { get; set; }
        public virtual DbSet<Blacklist> Blacklists { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Budget> Budgets { get; set; }
        public virtual DbSet<Cashb> Cashbs { get; set; }
        public virtual DbSet<Cashbook> Cashbooks { get; set; }
        public virtual DbSet<CashbookTransaction> CashbookTransactions { get; set; }
        public virtual DbSet<Cheque> Cheques { get; set; }
        public virtual DbSet<Combine> Combines { get; set; }
        public virtual DbSet<Contrib> Contribs { get; set; }
        public virtual DbSet<Ctr> Ctrs { get; set; }
        public virtual DbSet<Cub> Cubs { get; set; }
        public virtual DbSet<Curr> Currs { get; set; }
        public virtual DbSet<CustomerBalance> CustomerBalances { get; set; }
        public virtual DbSet<Customerbalanceold> Customerbalanceolds { get; set; }
        public virtual DbSet<DApprove1> DApprove1s { get; set; }
        public virtual DbSet<DApprove2> DApprove2s { get; set; }
        public virtual DbSet<DAssignmentVehicle> DAssignmentVehicles { get; set; }
        public virtual DbSet<DBank> DBanks { get; set; }
        public virtual DbSet<DBonu> DBonus { get; set; }
        public virtual DbSet<DBonus2> DBonus2s { get; set; }
        public virtual DbSet<DBranch> DBranch { get; set; }
        public virtual DbSet<DBankBranch> DBankBranch { get; set; }
        public virtual DbSet<DBranchProduct> DBranchProducts { get; set; }
        public virtual DbSet<DBranchsalesman> DBranchsalesmen { get; set; }
        public virtual DbSet<DCashPay> DCashPays { get; set; }
        public virtual DbSet<DCashShare> DCashShares { get; set; }
        public virtual DbSet<DChangepro> DChangepros { get; set; }
        public virtual DbSet<DCompany> DCompanies { get; set; }
        public virtual DbSet<DCostCent> DCostCents { get; set; }
        public virtual DbSet<DCtype> DCtypes { get; set; }
        public virtual DbSet<DDailySumm> DDailySumms { get; set; }
        public virtual DbSet<DDailySummary> DDailySummaries { get; set; }
        public virtual DbSet<DDailySummaryClerk> DDailySummaryClerks { get; set; }
        public virtual DbSet<DDcode> DDcodes { get; set; }
        public virtual DbSet<DDebtor> DDebtors { get; set; }
        public virtual DbSet<DDebtors2> DDebtors2s { get; set; }
        public virtual DbSet<DDebtorsparchase> DDebtorsparchases { get; set; }
        public virtual DbSet<DDebtorsparchases2> DDebtorsparchases2s { get; set; }
        public virtual DbSet<DDispatch> DDispatches { get; set; }
        public virtual DbSet<DDistrict> DDistricts { get; set; }
        public virtual DbSet<DGlposting> DGlpostings { get; set; }
        public virtual DbSet<DHeader> DHeaders { get; set; }
        public virtual DbSet<DIncomestate> DIncomestates { get; set; }
        public virtual DbSet<DInvoice> DInvoices { get; set; }
        public virtual DbSet<DLocation> DLocations { get; set; }
        public virtual DbSet<DLpo> DLpos { get; set; }
        public virtual DbSet<DMQsetting> DMQsettings { get; set; }
        public virtual DbSet<DMainAccount> DMainAccounts { get; set; }
        public virtual DbSet<DMaxShare> DMaxShares { get; set; }
        public virtual DbSet<DMilkBranch> DMilkBranches { get; set; }
        public virtual DbSet<DMilkControl> DMilkControls { get; set; }
        public virtual DbSet<DMilkControl1> DMilkControl1s { get; set; }
        public virtual DbSet<DMilkQuality> DMilkQualities { get; set; }
        public virtual DbSet<DMilkVehicle> DMilkVehicles { get; set; }
        public virtual DbSet<DMilkintake> DMilkintakes { get; set; }
        public virtual DbSet<DMilkintake1> DMilkintake1s { get; set; }
        public virtual DbSet<DMilkintake2> DMilkintake2s { get; set; }
        public virtual DbSet<DMilkintakeBackup> DMilkintakeBackups { get; set; }
        public virtual DbSet<DMilkintakechange> DMilkintakechanges { get; set; }
        public virtual DbSet<DMpayement> DMpayements { get; set; }
        public virtual DbSet<DOutlet> DOutlets { get; set; }
        public virtual DbSet<DOutletDispatch> DOutletDispatches { get; set; }
        public virtual DbSet<DOutletSale> DOutletSales { get; set; }
        public virtual DbSet<DOutletVehicle> DOutletVehicles { get; set; }
        public virtual DbSet<DOutletbranch> DOutletbranches { get; set; }
        public virtual DbSet<DOutletstock> DOutletstocks { get; set; }
        public virtual DbSet<DOutsale> DOutsales { get; set; }
        public virtual DbSet<DOutsalesb> DOutsalesbs { get; set; }
        public virtual DbSet<DPayment> DPayments { get; set; }
        public virtual DbSet<DPaymentReq> DPaymentReqs { get; set; }
        public virtual DbSet<DPayroll> DPayrolls { get; set; }
        public virtual DbSet<DPayrollCopy> DPayrollCopies { get; set; }
        public virtual DbSet<DPeriod> DPeriods { get; set; }
        public virtual DbSet<DPreSet> DPreSets { get; set; }
        public virtual DbSet<DPrice> DPrices { get; set; }
        public virtual DbSet<DPrice2> DPrice2s { get; set; }
        public virtual DbSet<DPriceBranch> DPriceBranches { get; set; }
        public virtual DbSet<DProductProcess> DProductProcesses { get; set; }
        public virtual DbSet<DQuality> DQualities { get; set; }
        public virtual DbSet<DReceipt> DReceipts { get; set; }
        public virtual DbSet<DRegistration> DRegistrations { get; set; }
        public virtual DbSet<DRequisition> DRequisitions { get; set; }
        public virtual DbSet<DSconribution> DSconributions { get; set; }
        public virtual DbSet<DShare> DShares { get; set; }
        public virtual DbSet<DSmscompany> DSmscompanies { get; set; }
        public virtual DbSet<DSmssetting> DSmssettings { get; set; }
        public virtual DbSet<DSupplier> DSuppliers { get; set; }
        public virtual DbSet<DSupplierDeduc> DSupplierDeducs { get; set; }
        public virtual DbSet<DSupplierStandingorder> DSupplierStandingorders { get; set; }
        public virtual DbSet<DSupplyDeducTran> DSupplyDeducTrans { get; set; }
        public virtual DbSet<DTblTrend> DTblTrends { get; set; }
        public virtual DbSet<DTmpEnquery> DTmpEnqueries { get; set; }
        public virtual DbSet<DTmpTransEnquery> DTmpTransEnqueries { get; set; }
        public virtual DbSet<DTransDetailed> DTransDetaileds { get; set; }
        public virtual DbSet<DTransFrate> DTransFrates { get; set; }
        public virtual DbSet<DTransMode> DTransModes { get; set; }
        public virtual DbSet<DTransport> DTransports { get; set; }
        public virtual DbSet<DTransportDeduc> DTransportDeducs { get; set; }
        public virtual DbSet<DTransportStandingorder> DTransportStandingorders { get; set; }
        public virtual DbSet<DTransporter> DTransporters { get; set; }
        public virtual DbSet<DTransportersPayRoll> DTransportersPayRolls { get; set; }
        public virtual DbSet<DType> DTypes { get; set; }
        public virtual DbSet<DVehicleTill> DVehicleTills { get; set; }
        public virtual DbSet<DailySupply> DailySupplies { get; set; }
        public virtual DbSet<Damagedstock> Damagedstocks { get; set; }
        public virtual DbSet<DeductionsQuery> DeductionsQueries { get; set; }
        public virtual DbSet<Depreciation> Depreciations { get; set; }
        public virtual DbSet<DetailedTransport> DetailedTransports { get; set; }
        public virtual DbSet<DisTmpActiveSup> DisTmpActiveSups { get; set; }
        public virtual DbSet<Drange> Dranges { get; set; }
        public virtual DbSet<Drawnstock> Drawnstocks { get; set; }
        public virtual DbSet<EasyMaPolicy> EasyMaPolicies { get; set; }
        public virtual DbSet<Gledger> Gledgers { get; set; }
        public virtual DbSet<Glsetup> Glsetups { get; set; }
        public virtual DbSet<Gltransaction> Gltransactions { get; set; }
        public virtual DbSet<Gltransactions2> Gltransactions2s { get; set; }
        public virtual DbSet<Journal> Journals { get; set; }
        public virtual DbSet<JournalType> JournalTypes { get; set; }
        public virtual DbSet<Journalslisting> Journalslistings { get; set; }
        public virtual DbSet<Kin> Kins { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<Login1> Logins1 { get; set; }
        public virtual DbSet<Matchedreport> Matchedreports { get; set; }
        public virtual DbSet<MaxShare> MaxShares { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Mpesab> Mpesabs { get; set; }
        public virtual DbSet<Param> Params { get; set; }
        public virtual DbSet<Passwordhistory> Passwordhistories { get; set; }
        public virtual DbSet<PaymentBooking> PaymentBookings { get; set; }
        public virtual DbSet<Payroll> Payrolls { get; set; }
        public virtual DbSet<Period> Periods { get; set; }
        public virtual DbSet<PettyCash> PettyCashes { get; set; }
        public virtual DbSet<Qbmp> Qbmps { get; set; }
        public virtual DbSet<Qsetup> Qsetups { get; set; }
        public virtual DbSet<Query1> Query1s { get; set; }
        public virtual DbSet<QueryBankSalary> QueryBankSalaries { get; set; }
        public virtual DbSet<QueryPayroll> QueryPayrolls { get; set; }
        public virtual DbSet<QueryPayrollQuery> QueryPayrollQueries { get; set; }
        public virtual DbSet<Rcpno> Rcpnos { get; set; }
        public virtual DbSet<ReceiptBooking> ReceiptBookings { get; set; }
        public virtual DbSet<Receiptno> Receiptnos { get; set; }
        public virtual DbSet<Reportpath> Reportpaths { get; set; }
        public virtual DbSet<ProductIntake> ProductIntake { get; set; }
        public virtual DbSet<Serialno> Serialnos { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<Share> Shares { get; set; }
        public virtual DbSet<SharesUpdate> SharesUpdates { get; set; }
        public virtual DbSet<Sisold> Sisolds { get; set; }
        public virtual DbSet<Smssubscription> Smssubscriptions { get; set; }
        public virtual DbSet<Sno> Snos { get; set; }
        public virtual DbSet<Staff1> Staffs { get; set; }
        public virtual DbSet<StaffPayroll> StaffPayrolls { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SwiftMessage> SwiftMessages { get; set; }
        public virtual DbSet<Sysparam> Sysparams { get; set; }
        public virtual DbSet<Tbbalance> Tbbalances { get; set; }
        public virtual DbSet<TblMenu> TblMenus { get; set; }
        public virtual DbSet<TblTfscostcentre> TblTfscostcentres { get; set; }
        public virtual DbSet<TblUsermenu> TblUsermenus { get; set; }
        public virtual DbSet<TchpRate> TchpRates { get; set; }
        public virtual DbSet<TempGlTransaction> TempGlTransactions { get; set; }
        public virtual DbSet<Tempcashbook> Tempcashbooks { get; set; }
        public virtual DbSet<Tempmemberstatement> Tempmemberstatements { get; set; }
        public virtual DbSet<Temttbbalance> Temttbbalances { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<Tmpaginganalysis> Tmpaginganalyses { get; set; }
        public virtual DbSet<Tmpcashbook> Tmpcashbooks { get; set; }
        public virtual DbSet<TotalShare> TotalShares { get; set; }
        public virtual DbSet<TrackChange> TrackChanges { get; set; }
        public virtual DbSet<Tran> Trans { get; set; }
        public virtual DbSet<TransCode> TransCodes { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Transport> Transports { get; set; }
        public virtual DbSet<Transporter> Transporters { get; set; }
        public virtual DbSet<Transporter1> Transporters1 { get; set; }
        public virtual DbSet<TransportersPayQuery> TransportersPayQueries { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAccount> UserAccounts { get; set; }
        public virtual DbSet<UserAccounts1> UserAccounts1s { get; set; }
        public virtual DbSet<Usergroup> Usergroups { get; set; }
        public virtual DbSet<Usergrp> Usergrps { get; set; }
        public virtual DbSet<View1> View1s { get; set; }
        public virtual DbSet<Vwpartly> Vwpartlies { get; set; }
        public virtual DbSet<Vwpartlytran> Vwpartlytrans { get; set; }
        public virtual DbSet<Vwshare> Vwshares { get; set; }
        public virtual DbSet<Vwsharebal> Vwsharebals { get; set; }
        public virtual DbSet<staff> staff { get; set; }
        public virtual DbSet<County> County { get; set; }
        public virtual DbSet<SubCounty> SubCounty { get; set; }
        public virtual DbSet<Ward> Ward { get; set; }
        public object DemoExcel { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-5GQU4IS;Database=MORINGA;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgClose>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_close");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("audit_date");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Craccno)
                    .HasMaxLength(50)
                    .HasColumnName("CRACCNO");

                entity.Property(e => e.DateEntered)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Date_Entered");

                entity.Property(e => e.Draccno)
                    .HasMaxLength(50)
                    .HasColumnName("DRACCNO");

                entity.Property(e => e.LastDUpdated)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Last_D_Updated");

                entity.Property(e => e.OBal).HasColumnName("o_bal");

                entity.Property(e => e.PCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.PName)
                    .HasMaxLength(200)
                    .HasColumnName("p_name");

                entity.Property(e => e.Pprice)
                    .HasColumnType("money")
                    .HasColumnName("pprice");

                entity.Property(e => e.SNo)
                    .HasMaxLength(50)
                    .HasColumnName("S_No");

                entity.Property(e => e.Seria).HasColumnName("seria");

                entity.Property(e => e.Serialized).HasMaxLength(50);

                entity.Property(e => e.Sprice)
                    .HasColumnType("money")
                    .HasColumnName("sprice");

                entity.Property(e => e.SupplierId)
                    .HasMaxLength(50)
                    .HasColumnName("SupplierID");

                entity.Property(e => e.Unserialized)
                    .HasMaxLength(50)
                    .HasColumnName("unserialized");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgClosingStock>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Ag_ClosingStock");

                entity.Property(e => e.Auditid).HasMaxLength(50);

                entity.Property(e => e.ChangeInStock).HasColumnType("money");

                entity.Property(e => e.ClosingStock).HasColumnType("money");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Opdate)
                    .HasColumnType("datetime")
                    .HasColumnName("opdate");

                entity.Property(e => e.OpenStock).HasColumnType("money");

                entity.Property(e => e.Opstock)
                    .HasColumnType("money")
                    .HasColumnName("opstock");

                entity.Property(e => e.Pname).HasColumnName("PName");

                entity.Property(e => e.Pprice).HasColumnType("money");

                entity.Property(e => e.Purchase).HasColumnType("money");

                entity.Property(e => e.Qty).HasColumnType("money");

                entity.Property(e => e.Sales)
                    .HasColumnType("money")
                    .HasColumnName("sales");

                entity.Property(e => e.Transdate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AgLpo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_lpo");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Duedate)
                    .HasColumnType("datetime")
                    .HasColumnName("duedate");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Itemname)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("itemname");

                entity.Property(e => e.Itemno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("itemno")
                    .IsFixedLength(true);

                entity.Property(e => e.Lpodate)
                    .HasColumnType("datetime")
                    .HasColumnName("lpodate");

                entity.Property(e => e.Lposerialno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lposerialno");

                entity.Property(e => e.Pono)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pono");

                entity.Property(e => e.Qty)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("QTY");

                entity.Property(e => e.Remarks)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("remarks");

                entity.Property(e => e.Vendor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("vendor");
            });

            modelBuilder.Entity<AgMoringaProduct>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_MoringaProducts");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Bname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BName");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LocalId).HasColumnName("local_id");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AgMoringaintake>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Moringaintake");

                entity.Property(e => e.Actual).HasColumnType("money");

                entity.Property(e => e.Auditedatetime)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("auditedatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Balance).HasColumnType("money");

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.System).HasColumnType("money");
            });

            modelBuilder.Entity<AgOpbal>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_opbal");

                entity.Property(e => e.PCode)
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.PName)
                    .HasMaxLength(200)
                    .HasColumnName("p_name");

                entity.Property(e => e.Transdate)
                    .HasColumnType("date")
                    .HasColumnName("transdate");
            });

            modelBuilder.Entity<AgPaging>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_paging");

                entity.Property(e => e.Audit)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("audit");

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdate");

                entity.Property(e => e.Dy).HasColumnName("dy");

                entity.Property(e => e.Grade)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("grade");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Ldate)
                    .HasColumnType("datetime")
                    .HasColumnName("ldate");

                entity.Property(e => e.Ltdate)
                    .HasColumnType("datetime")
                    .HasColumnName("ltdate");

                entity.Property(e => e.Pcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pcode");
            });

            modelBuilder.Entity<AgProduct>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Products");

                entity.Property(e => e.Ai).HasColumnName("AI");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("audit_date");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Craccno)
                    .HasMaxLength(50)
                    .HasColumnName("CRACCNO");

                entity.Property(e => e.DateEntered)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Date_Entered");

                entity.Property(e => e.Draccno)
                    .HasMaxLength(50)
                    .HasColumnName("DRACCNO");

                entity.Property(e => e.Expirydate).HasColumnType("smalldatetime");

                entity.Property(e => e.LastDUpdated)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Last_D_Updated");

                entity.Property(e => e.OBal).HasColumnName("o_bal");

                entity.Property(e => e.PCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.PName)
                    .HasMaxLength(200)
                    .HasColumnName("p_name");

                entity.Property(e => e.Pprice)
                    .HasColumnType("money")
                    .HasColumnName("pprice")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.Process1)
                    .HasColumnName("process1")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Process2)
                    .HasColumnName("process2")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Remarks)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'RE')");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.SNo)
                    .HasMaxLength(50)
                    .HasColumnName("S_No");

                entity.Property(e => e.Seria).HasColumnName("seria");

                entity.Property(e => e.Serialized).HasMaxLength(50);

                entity.Property(e => e.Sprice)
                    .HasColumnType("money")
                    .HasColumnName("sprice")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.SupplierId)
                    .HasMaxLength(50)
                    .HasColumnName("SupplierID");

                entity.Property(e => e.Unserialized)
                    .HasMaxLength(50)
                    .HasColumnName("unserialized");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgProducts1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Products1");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("audit_date");

                entity.Property(e => e.DateEntered)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Date_Entered");

                entity.Property(e => e.LastDUpdated)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Last_D_Updated");

                entity.Property(e => e.OBal).HasColumnName("o_bal");

                entity.Property(e => e.PCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.PName)
                    .HasMaxLength(200)
                    .HasColumnName("p_name");

                entity.Property(e => e.SNo)
                    .HasMaxLength(50)
                    .HasColumnName("S_No");

                entity.Property(e => e.Seria).HasColumnName("seria");

                entity.Property(e => e.Serialized).HasMaxLength(50);

                entity.Property(e => e.SupplierId)
                    .HasMaxLength(50)
                    .HasColumnName("SupplierID");

                entity.Property(e => e.Unserialized)
                    .HasMaxLength(50)
                    .HasColumnName("unserialized");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgProducts3>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Products3");

                entity.Property(e => e.Ai).HasColumnName("AI");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("audit_date");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.DateEntered)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Date_Entered");

                entity.Property(e => e.LastDUpdated)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Last_D_Updated");

                entity.Property(e => e.OBal).HasColumnName("o_bal");

                entity.Property(e => e.PCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.PName)
                    .HasMaxLength(200)
                    .HasColumnName("p_name");

                entity.Property(e => e.Pprice)
                    .HasColumnType("money")
                    .HasColumnName("pprice")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.SNo)
                    .HasMaxLength(50)
                    .HasColumnName("S_No");

                entity.Property(e => e.Seria).HasColumnName("seria");

                entity.Property(e => e.Serialized).HasMaxLength(50);

                entity.Property(e => e.Sprice)
                    .HasColumnType("money")
                    .HasColumnName("sprice")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.SupplierId)
                    .HasMaxLength(50)
                    .HasColumnName("SupplierID");

                entity.Property(e => e.Unserialized)
                    .HasMaxLength(50)
                    .HasColumnName("unserialized");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgProducts4>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Products4");

                entity.Property(e => e.Ai).HasColumnName("AI");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("audit_date");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Craccno)
                    .HasMaxLength(50)
                    .HasColumnName("CRACCNO");

                entity.Property(e => e.DateEntered)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Date_Entered");

                entity.Property(e => e.Draccno)
                    .HasMaxLength(50)
                    .HasColumnName("DRACCNO");

                entity.Property(e => e.LastDUpdated)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Last_D_Updated");

                entity.Property(e => e.OBal).HasColumnName("o_bal");

                entity.Property(e => e.PCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.PName)
                    .HasMaxLength(200)
                    .HasColumnName("p_name");

                entity.Property(e => e.Pprice)
                    .HasColumnType("money")
                    .HasColumnName("pprice");

                entity.Property(e => e.SNo).HasColumnName("S_No");

                entity.Property(e => e.Seria).HasColumnName("seria");

                entity.Property(e => e.Serialized).HasMaxLength(50);

                entity.Property(e => e.Sprice)
                    .HasColumnType("money")
                    .HasColumnName("sprice");

                entity.Property(e => e.SupplierId)
                    .HasMaxLength(50)
                    .HasColumnName("SupplierID");

                entity.Property(e => e.Unserialized)
                    .HasMaxLength(50)
                    .HasColumnName("unserialized");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgProducts5>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Products5");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("datetime")
                    .HasColumnName("audit_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Craccno)
                    .HasMaxLength(50)
                    .HasColumnName("CRACCNO");

                entity.Property(e => e.DateEntered)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Date_Entered");

                entity.Property(e => e.Draccno)
                    .HasMaxLength(50)
                    .HasColumnName("DRACCNO");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.LastDUpdated)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Last_D_Updated");

                entity.Property(e => e.Narration).HasMaxLength(50);

                entity.Property(e => e.OBal).HasColumnName("o_bal");

                entity.Property(e => e.PCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.PName)
                    .HasMaxLength(200)
                    .HasColumnName("p_name");

                entity.Property(e => e.Pprice)
                    .HasColumnType("money")
                    .HasColumnName("pprice");

                entity.Property(e => e.SNo)
                    .HasMaxLength(50)
                    .HasColumnName("S_No");

                entity.Property(e => e.Seria).HasColumnName("seria");

                entity.Property(e => e.Serialized).HasMaxLength(50);

                entity.Property(e => e.Sprice)
                    .HasColumnType("money")
                    .HasColumnName("sprice");

                entity.Property(e => e.SupplierId)
                    .HasMaxLength(50)
                    .HasColumnName("SupplierID");

                entity.Property(e => e.Unserialized)
                    .HasMaxLength(50)
                    .HasColumnName("unserialized");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgReceipt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Receipts");

                entity.Property(e => e.Ai).HasColumnName("AI");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("datetime")
                    .HasColumnName("audit_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Bprice)
                    .HasColumnType("money")
                    .HasColumnName("BPRICE");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Cash).HasDefaultValueSql("((0))");

                entity.Property(e => e.Completed).HasDefaultValueSql("((1))");

                entity.Property(e => e.Idno).HasMaxLength(50);

                entity.Property(e => e.Mobile)
                    .HasMaxLength(50)
                    .HasColumnName("mobile");

                entity.Property(e => e.PCode)
                    .HasMaxLength(50)
                    .HasColumnName("P_code");

                entity.Property(e => e.Paid)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("R_id");

                entity.Property(e => e.RNo)
                    .HasMaxLength(50)
                    .HasColumnName("R_No");

                entity.Property(e => e.Remarks).HasMaxLength(200);

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.SBal).HasColumnName("S_Bal");

                entity.Property(e => e.SNo)
                    .HasMaxLength(50)
                    .HasColumnName("S_No");

                entity.Property(e => e.Salesrep)
                    .HasMaxLength(50)
                    .HasColumnName("salesrep");

                entity.Property(e => e.Sno1)
                    .HasMaxLength(50)
                    .HasColumnName("SNO");

                entity.Property(e => e.Sprice)
                    .HasColumnType("money")
                    .HasColumnName("SPRICE");

                entity.Property(e => e.TDate)
                    .HasColumnType("datetime")
                    .HasColumnName("T_Date");

                entity.Property(e => e.Transby).HasMaxLength(50);

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgReceipts1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Receipts1");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("audit_date");

                entity.Property(e => e.PCode)
                    .HasMaxLength(50)
                    .HasColumnName("P_code");

                entity.Property(e => e.RId).HasColumnName("R_id");

                entity.Property(e => e.RNo).HasColumnName("R_No");

                entity.Property(e => e.SBal).HasColumnName("S_Bal");

                entity.Property(e => e.SNo)
                    .HasMaxLength(50)
                    .HasColumnName("S_No");

                entity.Property(e => e.TDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("T_Date");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgReceipts3>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Receipts3");

                entity.Property(e => e.Ai).HasColumnName("AI");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("datetime")
                    .HasColumnName("audit_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Cash).HasDefaultValueSql("((0))");

                entity.Property(e => e.Idno).HasMaxLength(50);

                entity.Property(e => e.Mobile)
                    .HasMaxLength(50)
                    .HasColumnName("mobile");

                entity.Property(e => e.PCode)
                    .HasMaxLength(50)
                    .HasColumnName("P_code");

                entity.Property(e => e.RId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("R_id");

                entity.Property(e => e.RNo)
                    .HasMaxLength(50)
                    .HasColumnName("R_No");

                entity.Property(e => e.Remarks).HasMaxLength(50);

                entity.Property(e => e.SBal).HasColumnName("S_Bal");

                entity.Property(e => e.SNo)
                    .HasMaxLength(50)
                    .HasColumnName("S_No");

                entity.Property(e => e.Salesrep)
                    .HasMaxLength(50)
                    .HasColumnName("salesrep");

                entity.Property(e => e.Sno1)
                    .HasMaxLength(50)
                    .HasColumnName("SNO");

                entity.Property(e => e.TDate)
                    .HasColumnType("datetime")
                    .HasColumnName("T_Date");

                entity.Property(e => e.Transby).HasMaxLength(50);

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgReceiptsEnqury>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_ReceiptsEnqury");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Paid)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Pcode).HasMaxLength(10);

                entity.Property(e => e.RId).HasColumnName("R_id");

                entity.Property(e => e.Salesrep)
                    .HasMaxLength(50)
                    .HasColumnName("salesrep");
            });

            modelBuilder.Entity<AgReceiptsProcess>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_ReceiptsProcess");

                entity.Property(e => e.BPro1).HasColumnName("bPro1");

                entity.Property(e => e.BPro2).HasColumnName("bPro2");

                entity.Property(e => e.BPro3).HasColumnName("bPro3");

                entity.Property(e => e.BPro4).HasColumnName("bPro4");

                entity.Property(e => e.BPro5).HasColumnName("bPro5");

                entity.Property(e => e.BPro6).HasColumnName("bPro6");

                entity.Property(e => e.BPro7).HasColumnName("bPro7");

                entity.Property(e => e.BPro8).HasColumnName("bPro8");

                entity.Property(e => e.BPro9).HasColumnName("bPro9");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.SPro1).HasColumnName("sPro1");

                entity.Property(e => e.SPro2).HasColumnName("sPro2");

                entity.Property(e => e.SPro3).HasColumnName("sPro3");

                entity.Property(e => e.SPro4).HasColumnName("sPro4");

                entity.Property(e => e.SPro5).HasColumnName("sPro5");

                entity.Property(e => e.SPro6).HasColumnName("sPro6");

                entity.Property(e => e.SPro7).HasColumnName("sPro7");

                entity.Property(e => e.SPro8).HasColumnName("sPro8");

                entity.Property(e => e.SPro9).HasColumnName("sPro9");

                entity.Property(e => e.Sno)
                    .HasMaxLength(50)
                    .HasColumnName("SNo");
            });

            modelBuilder.Entity<AgReceiptsalesrep>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Receiptsalesrep");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("datetime")
                    .HasColumnName("audit_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Paid)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RId).HasColumnName("R_id");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgSale>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_sales");

                entity.Property(e => e.Balance).HasColumnType("money");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Openning).HasColumnType("money");

                entity.Property(e => e.Pcode).HasMaxLength(50);

                entity.Property(e => e.Pname)
                    .HasMaxLength(500)
                    .HasColumnName("pname");

                entity.Property(e => e.Purchases).HasColumnType("money");

                entity.Property(e => e.Sales).HasColumnType("money");
            });

            modelBuilder.Entity<AgStockbalance>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_stockbalance");

                entity.Property(e => e.Ai).HasColumnName("AI");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("audit_date");

                entity.Property(e => e.Auditdate1)
                    .HasColumnType("datetime")
                    .HasColumnName("Auditdate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Changeinstock)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("changeinstock");

                entity.Property(e => e.Companyid).HasColumnName("companyid");

                entity.Property(e => e.Expirydate).HasColumnType("smalldatetime");

                entity.Property(e => e.Openningstock)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("openningstock");

                entity.Property(e => e.PCode)
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.Pprice)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.Process1)
                    .HasColumnName("process1")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Process2)
                    .HasColumnName("process2")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Productname).HasColumnName("productname");

                entity.Property(e => e.RNo)
                    .HasMaxLength(50)
                    .HasColumnName("R_NO");

                entity.Property(e => e.Remarks).HasMaxLength(50);

                entity.Property(e => e.Rlevel)
                    .HasColumnName("RLevel")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.SNo)
                    .HasMaxLength(50)
                    .HasColumnName("S_NO");

                entity.Property(e => e.Sprice)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.Stockbalance).HasColumnName("stockbalance");

                entity.Property(e => e.Subclass)
                    .HasMaxLength(50)
                    .HasColumnName("subclass");

                entity.Property(e => e.Trackid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("trackid");

                entity.Property(e => e.Transdate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("transdate");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgStockbalance1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_stockbalance1");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("audit_date");

                entity.Property(e => e.Changeinstock)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("changeinstock");

                entity.Property(e => e.Companyid).HasColumnName("companyid");

                entity.Property(e => e.Openningstock)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("openningstock");

                entity.Property(e => e.PCode)
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.Productname)
                    .HasMaxLength(50)
                    .HasColumnName("productname");

                entity.Property(e => e.RNo)
                    .HasMaxLength(50)
                    .HasColumnName("R_NO");

                entity.Property(e => e.SNo).HasColumnName("S_NO");

                entity.Property(e => e.Stockbalance).HasColumnName("stockbalance");

                entity.Property(e => e.Subclass)
                    .HasMaxLength(50)
                    .HasColumnName("subclass");

                entity.Property(e => e.Trackid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("trackid");

                entity.Property(e => e.Transdate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("transdate");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<AgSupplier>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_supplier");

                entity.Property(e => e.Address1)
                    .HasMaxLength(50)
                    .HasColumnName("address1");

                entity.Property(e => e.Address2)
                    .HasMaxLength(50)
                    .HasColumnName("address2");

                entity.Property(e => e.Address3)
                    .HasMaxLength(50)
                    .HasColumnName("address3");

                entity.Property(e => e.EmailAdd)
                    .HasMaxLength(50)
                    .HasColumnName("email_add");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.SuppId)
                    .HasMaxLength(50)
                    .HasColumnName("Supp_id");

                entity.Property(e => e.SuppName)
                    .HasMaxLength(100)
                    .HasColumnName("Supp_name");

                entity.Property(e => e.Town)
                    .HasMaxLength(50)
                    .HasColumnName("town");
            });

            modelBuilder.Entity<AgSupplier1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ag_Supplier1");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ContactPerson).HasMaxLength(50);

                entity.Property(e => e.ContactTitle).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Fax).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.SupplierId)
                    .HasMaxLength(50)
                    .HasColumnName("SupplierID");
            });

            modelBuilder.Entity<Ap>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AP");

                entity.Property(e => e.Accno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("accno")
                    .IsFixedLength(true);

                entity.Property(e => e.Approvedby)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("auditid")
                    .IsFixedLength(true);

                entity.Property(e => e.CheckedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("checked_by");

                entity.Property(e => e.ChequeDate).HasColumnType("datetime");

                entity.Property(e => e.Chequeno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("chequeno")
                    .IsFixedLength(true);

                entity.Property(e => e.Chequestatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("chequestatus");

                entity.Property(e => e.Curr)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("curr")
                    .IsFixedLength(true);

                entity.Property(e => e.DateCollected)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Collected");

                entity.Property(e => e.Exchangerate).HasColumnName("exchangerate");

                entity.Property(e => e.FAccNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("F_AccNo")
                    .IsFixedLength(true);

                entity.Property(e => e.Idno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("IDNo");

                entity.Property(e => e.InvNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("inv_no")
                    .IsFixedLength(true);

                entity.Property(e => e.PAccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("P_accno")
                    .IsFixedLength(true);

                entity.Property(e => e.PAmount)
                    .HasColumnType("money")
                    .HasColumnName("p_amount");

                entity.Property(e => e.PDate)
                    .HasColumnType("datetime")
                    .HasColumnName("p_date");

                entity.Property(e => e.PNo).HasColumnName("P_No");

                entity.Property(e => e.Paidto)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("paidto");

                entity.Property(e => e.Particulars)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("particulars");

                entity.Property(e => e.Paymentmode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("paymentmode");

                entity.Property(e => e.Pid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("pid");

                entity.Property(e => e.Posted)
                    .HasColumnName("posted")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Purpose)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Receivedby)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Vat)
                    .HasColumnType("money")
                    .HasColumnName("VAT");

                entity.Property(e => e.VatAccno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Vat_accno");
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(e => e.AssetsNo);

                entity.ToTable("assets");

                entity.Property(e => e.AssetsNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Accdepreciation).HasColumnType("money");

                entity.Property(e => e.Accno).HasMaxLength(50);

                entity.Property(e => e.AssetType).HasMaxLength(50);

                entity.Property(e => e.AssetsName).HasMaxLength(50);

                entity.Property(e => e.AssetserialNo).HasMaxLength(50);

                entity.Property(e => e.Assetsid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("assetsid");

                entity.Property(e => e.Currentvalue).HasColumnType("money");

                entity.Property(e => e.DAmount)
                    .HasColumnType("money")
                    .HasColumnName("d_amount")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Datebought)
                    .HasColumnType("datetime")
                    .HasColumnName("datebought");

                entity.Property(e => e.Disposaldate)
                    .HasColumnType("datetime")
                    .HasColumnName("disposaldate");

                entity.Property(e => e.Month).HasColumnName("month");

                entity.Property(e => e.Notes)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("notes");

                entity.Property(e => e.Nrvbf)
                    .HasColumnType("money")
                    .HasColumnName("NRVBF");

                entity.Property(e => e.PurchasePrice).HasColumnType("money");

                entity.Property(e => e.Revaluation)
                    .HasColumnType("money")
                    .HasColumnName("revaluation");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Transdate).HasColumnType("datetime");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<Assetcode>(entity =>
            {
                entity.HasKey(e => e.Assetcode1);

                entity.ToTable("assetcode");

                entity.Property(e => e.Assetcode1)
                    .HasMaxLength(50)
                    .HasColumnName("assetcode");

                entity.Property(e => e.Assetid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("assetid");

                entity.Property(e => e.Assetname)
                    .HasMaxLength(50)
                    .HasColumnName("assetname");

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.Remarks)
                    .HasMaxLength(50)
                    .HasColumnName("remarks");
            });

            modelBuilder.Entity<AssetsRegister>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("assets_register");

                entity.Property(e => e.Accno)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("accno");

                entity.Property(e => e.Asid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("asid");

                entity.Property(e => e.Assetcode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("assetcode");

                entity.Property(e => e.Assetname)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("assetname");

                entity.Property(e => e.ContraAccno)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CurrentValue).HasColumnType("money");

                entity.Property(e => e.Deprate).HasColumnName("deprate");

                entity.Property(e => e.Pdate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PurchasePrice).HasColumnType("money");

                entity.Property(e => e.SerialNo).HasMaxLength(50);
            });

            modelBuilder.Entity<Assetstran>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("assetstrans");

                entity.Property(e => e.AmountdepVal)
                    .HasColumnType("money")
                    .HasColumnName("Amountdep_val");

                entity.Property(e => e.Assetcode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Assetname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Assettransid).ValueGeneratedOnAdd();

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("auditid")
                    .IsFixedLength(true);

                entity.Property(e => e.DepVal).HasColumnName("Dep_val");

                entity.Property(e => e.Mmonth).HasColumnName("mmonth");

                entity.Property(e => e.Nrv)
                    .HasColumnType("money")
                    .HasColumnName("NRV");

                entity.Property(e => e.Posted).HasColumnName("posted");

                entity.Property(e => e.Quaters).HasColumnName("quaters");

                entity.Property(e => e.Transdate)
                    .HasColumnType("datetime")
                    .HasColumnName("transdate");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<Assetstrans1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("assetstrans1");

                entity.Property(e => e.AmountdepVal)
                    .HasColumnType("money")
                    .HasColumnName("Amountdep_val");

                entity.Property(e => e.Assetcode).HasMaxLength(50);

                entity.Property(e => e.Assetname).HasMaxLength(50);

                entity.Property(e => e.DepVal).HasColumnName("Dep_val");

                entity.Property(e => e.Nrv)
                    .HasColumnType("money")
                    .HasColumnName("NRV");

                entity.Property(e => e.Quaters).HasColumnName("quaters");

                entity.Property(e => e.Transdate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("transdate");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<Assetstrans2>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("assetstrans2");

                entity.Property(e => e.AmountdepVal)
                    .HasColumnType("money")
                    .HasColumnName("Amountdep_val");

                entity.Property(e => e.Assetcode).HasMaxLength(50);

                entity.Property(e => e.Assetname).HasMaxLength(50);

                entity.Property(e => e.DepVal).HasColumnName("Dep_val");

                entity.Property(e => e.Nrv)
                    .HasColumnType("money")
                    .HasColumnName("NRV");

                entity.Property(e => e.Quaters).HasColumnName("quaters");

                entity.Property(e => e.Transdate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("transdate");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<Audittable>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AUDITTABLE");

                entity.Property(e => e.Auditid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("AUDITID");

                entity.Property(e => e.LoginDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LoginTime)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LogoffTime)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Moduleid)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("moduleid")
                    .IsFixedLength(true);

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserTransaction)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Audittran>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AUDITTRANS");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime).HasColumnType("datetime");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransDescription)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TransId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("TransID");

                entity.Property(e => e.TransTable)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Audittrans1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AUDITTRANS1");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditid)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Audittime).HasColumnType("datetime");

                entity.Property(e => e.Transdate).HasColumnType("datetime");

                entity.Property(e => e.Transdescription)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Transtable)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<B2cdisbursementResponse>(entity =>
            {
                entity.ToTable("B2CDisbursementResponse");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.B2cutilityAccountAvailableFunds)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("B2CUtilityAccountAvailableFunds");

                entity.Property(e => e.ConversationId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ConversationID");

                entity.Property(e => e.OriginatorConversationId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("OriginatorConversationID");

                entity.Property(e => e.PhoneNo2).HasMaxLength(50);

                entity.Property(e => e.ReceiverPartyPublicName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ResultDesc)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.Run2).HasDefaultValueSql("((0))");

                entity.Property(e => e.TransacionReceipt)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionCompletedDateTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<B2cpaymentDummy>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("B2CPaymentDummy");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DatePosted).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Location).HasMaxLength(50);

                entity.Property(e => e.Names)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OtherParyInfo).HasMaxLength(50);

                entity.Property(e => e.PhoneNo).HasMaxLength(50);

                entity.Property(e => e.ReceiptNo).HasMaxLength(50);

                entity.Property(e => e.Sno)
                    .HasMaxLength(50)
                    .HasColumnName("SNo");

                entity.Property(e => e.Status1).HasDefaultValueSql("((0))");

                entity.Property(e => e.TransactionStatus).HasMaxLength(50);

                entity.Property(e => e.User1)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BANKS");

                entity.Property(e => e.AccNo)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.AccType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Address).HasMaxLength(30);

                entity.Property(e => e.AuditId)
                    .HasMaxLength(10)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime).HasColumnType("smalldatetime");

                entity.Property(e => e.BankAccno).HasMaxLength(50);

                entity.Property(e => e.BankCode).HasMaxLength(10);

                entity.Property(e => e.BankName).HasMaxLength(50);

                entity.Property(e => e.BranchName).HasMaxLength(50);

                entity.Property(e => e.Telephone).HasMaxLength(15);
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BankAccount");

                entity.Property(e => e.AccName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Chequeno)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("chequeno");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Naration)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Piro)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PIRO");

                entity.Property(e => e.Pvcno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Transdate)
                    .HasColumnType("datetime")
                    .HasColumnName("transdate");
            });

            modelBuilder.Entity<BankRecon>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BankRecon");

                entity.Property(e => e.AccNo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CashBookBal).HasColumnType("money");

                entity.Property(e => e.DirectCredits).HasColumnType("money");

                entity.Property(e => e.DirectDebits).HasColumnType("money");

                entity.Property(e => e.OpeningBal).HasColumnType("money");

                entity.Property(e => e.OpeningBalDate).HasColumnType("datetime");

                entity.Property(e => e.Payments).HasColumnType("money");

                entity.Property(e => e.Receipts).HasColumnType("money");

                entity.Property(e => e.ReconDate).HasColumnType("datetime");

                entity.Property(e => e.ReconId).ValueGeneratedOnAdd();

                entity.Property(e => e.StatementBal).HasColumnType("money");

                entity.Property(e => e.UnCredited).HasColumnType("money");

                entity.Property(e => e.Unpresented).HasColumnType("money");
            });

            modelBuilder.Entity<Banks1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Banks1");

                entity.Property(e => e.BankName).HasMaxLength(255);
            });

            modelBuilder.Entity<Barcodedsale>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("barcodedsales");

                entity.Property(e => e.Batchno)
                    .HasMaxLength(50)
                    .HasColumnName("batchno");

                entity.Property(e => e.Chequeno)
                    .HasMaxLength(50)
                    .HasColumnName("chequeno");

                entity.Property(e => e.Paymenttype)
                    .HasMaxLength(50)
                    .HasColumnName("paymenttype");

                entity.Property(e => e.Sellingprice)
                    .HasColumnType("money")
                    .HasColumnName("sellingprice");

                entity.Property(e => e.Serialcode)
                    .HasMaxLength(100)
                    .HasColumnName("serialcode");

                entity.Property(e => e.Serialid).HasColumnName("serialid");

                entity.Property(e => e.Stockname)
                    .HasMaxLength(50)
                    .HasColumnName("stockname");

                entity.Property(e => e.Totalamount)
                    .HasColumnType("money")
                    .HasColumnName("totalamount");

                entity.Property(e => e.Transdate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("transdate");

                entity.Property(e => e.Units).HasColumnName("units");
            });

            modelBuilder.Entity<Barcodeitem>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("barcodeitems");

                entity.Property(e => e.Category)
                    .HasMaxLength(50)
                    .HasColumnName("category");

                entity.Property(e => e.Purchaseprice)
                    .HasColumnType("money")
                    .HasColumnName("purchaseprice");

                entity.Property(e => e.Receiptdate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("receiptdate");

                entity.Property(e => e.Saleprice)
                    .HasColumnType("money")
                    .HasColumnName("saleprice");

                entity.Property(e => e.Serialcode)
                    .HasMaxLength(50)
                    .HasColumnName("serialcode");

                entity.Property(e => e.Serialid).HasColumnName("serialid");

                entity.Property(e => e.Stockcode)
                    .HasMaxLength(50)
                    .HasColumnName("stockcode");

                entity.Property(e => e.Stockname)
                    .HasMaxLength(50)
                    .HasColumnName("stockname");

                entity.Property(e => e.Units)
                    .HasMaxLength(50)
                    .HasColumnName("units");

                entity.Property(e => e.Vatrate).HasColumnName("vatrate");
            });

            modelBuilder.Entity<Blacklist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Blacklist");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Phoneno)
                    .HasMaxLength(50)
                    .HasColumnName("phoneno");

                entity.Property(e => e.Remarks).HasMaxLength(50);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BOOKINGS");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ChequeNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CrAccNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DocPosted).HasColumnName("doc_posted");

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.DrAccNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Dregard)
                    .HasColumnName("dregard")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Glpost).HasColumnName("glpost");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransDescript)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BUDGETS");

                entity.Property(e => e.Accno).HasMaxLength(20);

                entity.Property(e => e.Actual).HasColumnType("money");

                entity.Property(e => e.BudgetDate).HasColumnType("datetime");

                entity.Property(e => e.Budgetted).HasColumnType("money");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Mmonth).HasColumnName("mmonth");

                entity.Property(e => e.Variance).HasColumnType("money");

                entity.Property(e => e.Yyear).HasColumnName("yyear");
            });

            modelBuilder.Entity<Cashb>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CASHB");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransType)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Cashbook>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CASHBOOK");

                entity.Property(e => e.AccNo).HasMaxLength(50);

                entity.Property(e => e.AccNoCr).HasMaxLength(50);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(100)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime).HasColumnType("datetime");

                entity.Property(e => e.CashId).HasColumnName("CashID");

                entity.Property(e => e.ChequeNo).HasMaxLength(50);

                entity.Property(e => e.MemberNo).HasMaxLength(50);

                entity.Property(e => e.ReceiptNo).HasMaxLength(50);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransDescript).HasMaxLength(200);

                entity.Property(e => e.TransId)
                    .HasMaxLength(50)
                    .HasColumnName("TransID");
            });

            modelBuilder.Entity<CashbookTransaction>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CashbookTransaction");

                entity.Property(e => e.Accno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.CashId).ValueGeneratedOnAdd();

                entity.Property(e => e.Chequeno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdNo).HasDefaultValueSql("(4)");

                entity.Property(e => e.Pd)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Reconciled).HasDefaultValueSql("(0)");

                entity.Property(e => e.Transby)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Transdate).HasColumnType("datetime");

                entity.Property(e => e.Transdescription)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Transtype)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Vno)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Cheque>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CHEQUES");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(10)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime).HasColumnType("datetime");

                entity.Property(e => e.ChequeId).HasColumnName("ChequeID");

                entity.Property(e => e.ChequeNo).HasMaxLength(30);

                entity.Property(e => e.ClerkName).HasMaxLength(45);

                entity.Property(e => e.ClerkStaffNo).HasMaxLength(20);

                entity.Property(e => e.CollectorId)
                    .HasMaxLength(10)
                    .HasColumnName("CollectorID");

                entity.Property(e => e.CollectorName).HasMaxLength(100);

                entity.Property(e => e.ContraAcc)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateIssued).HasColumnType("datetime");

                entity.Property(e => e.Dregard).HasColumnName("dregard");

                entity.Property(e => e.IntAmount).HasColumnType("money");

                entity.Property(e => e.LoanAcc)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LoanNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OffsetAmount).HasColumnType("money");

                entity.Property(e => e.Premium).HasColumnType("money");

                entity.Property(e => e.PremiumAcc)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Reasons).HasMaxLength(50);

                entity.Property(e => e.Remarks).HasMaxLength(20);

                entity.Property(e => e.Status).HasMaxLength(15);
            });

            modelBuilder.Entity<Combine>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("COMBINE");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .HasColumnName("CODE")
                    .IsFixedLength(true);

                entity.Property(e => e.Sno)
                    .HasMaxLength(10)
                    .HasColumnName("SNO")
                    .IsFixedLength(true);

                entity.Property(e => e.Status).HasColumnName("STATUS");
            });

            modelBuilder.Entity<Contrib>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CONTRIB");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(200)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ChequeNo).HasMaxLength(500);

                entity.Property(e => e.Commission).HasColumnType("money");

                entity.Property(e => e.ContrDate).HasColumnType("date");

                entity.Property(e => e.Fperiod).HasColumnName("fperiod");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.IntRate).HasColumnName("intRate");

                entity.Property(e => e.Interest).HasColumnType("money");

                entity.Property(e => e.Locked).HasMaxLength(3);

                entity.Property(e => e.Maturitydate)
                    .HasColumnType("datetime")
                    .HasColumnName("maturitydate");

                entity.Property(e => e.MemberNo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Posted).HasMaxLength(3);

                entity.Property(e => e.ReceiptNo).HasMaxLength(50);

                entity.Property(e => e.Remarks).HasMaxLength(300);

                entity.Property(e => e.ShareBal).HasColumnType("money");

                entity.Property(e => e.Sharescode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("sharescode");

                entity.Property(e => e.TransBy).HasMaxLength(100);

                entity.Property(e => e.TransactionNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Used).HasColumnName("used");
            });

            modelBuilder.Entity<Ctr>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Ctr");

                entity.Property(e => e.Autodept)
                    .HasMaxLength(50)
                    .HasColumnName("autodept");

                entity.Property(e => e.Autoissue)
                    .HasMaxLength(50)
                    .HasColumnName("autoissue");

                entity.Property(e => e.Autono)
                    .HasMaxLength(50)
                    .HasColumnName("autono");

                entity.Property(e => e.Autoprod)
                    .HasMaxLength(50)
                    .HasColumnName("autoprod");

                entity.Property(e => e.Autorec)
                    .HasMaxLength(50)
                    .HasColumnName("autorec");
            });

            modelBuilder.Entity<Cub>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CUB");

                entity.Property(e => e.AccNo)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.AccountName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Actualbalance)
                    .HasColumnType("money")
                    .HasColumnName("actualbalance");

                entity.Property(e => e.Adbal).HasColumnType("money");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdate).HasColumnType("datetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasColumnName("AUDITID");

                entity.Property(e => e.AvailableBalance).HasColumnType("money");

                entity.Property(e => e.BranchAccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("branch_accno")
                    .IsFixedLength(true);

                entity.Property(e => e.Branchcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("branchcode")
                    .IsFixedLength(true);

                entity.Property(e => e.Category)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ChequeNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Closed).HasColumnName("CLOSED");

                entity.Property(e => e.Commission).HasColumnType("money");

                entity.Property(e => e.Controlacc)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("controlacc");

                entity.Property(e => e.Cuid).ValueGeneratedOnAdd();

                entity.Property(e => e.Curr)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("curr")
                    .IsFixedLength(true);

                entity.Property(e => e.Don)
                    .HasColumnType("datetime")
                    .HasColumnName("DON");

                entity.Property(e => e.Edon)
                    .HasColumnType("datetime")
                    .HasColumnName("EDON");

                entity.Property(e => e.Excess)
                    .HasColumnType("money")
                    .HasColumnName("excess");

                entity.Property(e => e.Frozen).HasColumnName("FROZEN");

                entity.Property(e => e.Graceperiod).HasColumnName("graceperiod");

                entity.Property(e => e.Hassubledgers).HasColumnName("hassubledgers");

                entity.Property(e => e.Hs)
                    .HasColumnType("money")
                    .HasColumnName("hs");

                entity.Property(e => e.Idno)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("IDNo");

                entity.Property(e => e.Issubledger).HasColumnName("issubledger");

                entity.Property(e => e.Ledger).HasColumnName("ledger");

                entity.Property(e => e.Limitexpirydate)
                    .HasColumnType("datetime")
                    .HasColumnName("limitexpirydate");

                entity.Property(e => e.Loans)
                    .HasColumnType("money")
                    .HasColumnName("loans");

                entity.Property(e => e.Main).HasColumnName("main");

                entity.Property(e => e.MainAccHeader)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("main_acc_header");

                entity.Property(e => e.MainAccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("main_accno")
                    .IsFixedLength(true);

                entity.Property(e => e.MemberNo)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Moduleid)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("moduleid")
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nhif).HasColumnType("money");

                entity.Property(e => e.Nomi1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NOMI1");

                entity.Property(e => e.Nomi1id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NOMI1ID");

                entity.Property(e => e.Nomi2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NOMI2");

                entity.Property(e => e.Nomi2id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NOMI2ID");

                entity.Property(e => e.Nomi3)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NOMI3");

                entity.Property(e => e.Nomi3id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NOMI3ID");

                entity.Property(e => e.Notes).HasColumnType("text");

                entity.Property(e => e.Ordshares)
                    .HasColumnType("money")
                    .HasColumnName("ordshares");

                entity.Property(e => e.Payno)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Period)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("period")
                    .IsFixedLength(true);

                entity.Property(e => e.Picture)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("picture");

                entity.Property(e => e.Picture1)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("picture1");

                entity.Property(e => e.Regdate).HasColumnType("datetime");

                entity.Property(e => e.Sex)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("sex")
                    .IsFixedLength(true);

                entity.Property(e => e.Sig1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SIG1");

                entity.Property(e => e.Sig1id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SIG1ID");

                entity.Property(e => e.Sig2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SIG2");

                entity.Property(e => e.Sig2id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SIG2ID");

                entity.Property(e => e.Sig3)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SIG3");

                entity.Property(e => e.Sig3id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SIG3ID");

                entity.Property(e => e.Sig4)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SIG4");

                entity.Property(e => e.Sig4id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SIG4ID");

                entity.Property(e => e.Signature)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("signature");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Tgno1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("tgno1")
                    .IsFixedLength(true);

                entity.Property(e => e.Tgno2)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("tgno2")
                    .IsFixedLength(true);

                entity.Property(e => e.Tgno3)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("tgno3")
                    .IsFixedLength(true);

                entity.Property(e => e.Transdate).HasColumnType("datetime");

                entity.Property(e => e.Transdescription)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Transtype)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Unitmarked)
                    .HasColumnType("money")
                    .HasColumnName("unitmarked");

                entity.Property(e => e.Vno)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("VNO");
            });

            modelBuilder.Entity<Curr>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Curr");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("auditid")
                    .IsFixedLength(true);

                entity.Property(e => e.CurrCode)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DecimalSeparator)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.NegativeDisplay)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Rateaganistsource).HasColumnName("rateaganistsource");

                entity.Property(e => e.Symbol)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Symbolposition)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ThousandSeparator)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<CustomerBalance>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CustomerBalance");

                entity.Property(e => e.AccName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.AccNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("AccNO");

                entity.Property(e => e.Accd)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("accd")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Actualbalance)
                    .HasColumnType("money")
                    .HasColumnName("actualbalance");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(70)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Admin')");

                entity.Property(e => e.AvailableBalance)
                    .HasColumnType("money")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Bcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("bcode")
                    .IsFixedLength(true);

                entity.Property(e => e.ChequeNo)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Commission)
                    .HasColumnType("money")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.CustomerNo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Customerbalanceid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("customerbalanceid");

                entity.Property(e => e.Dregard).HasColumnName("dregard");

                entity.Property(e => e.Idno)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("IDNo");

                entity.Property(e => e.Moduleid)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("moduleid")
                    .IsFixedLength(true);

                entity.Property(e => e.PayrollNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Period)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Rebuild)
                    .HasColumnName("rebuild")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.Reconciled).HasColumnName("reconciled");

                entity.Property(e => e.SCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("s_code")
                    .IsFixedLength(true);

                entity.Property(e => e.TransDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TransDescription)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.TransType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("transType")
                    .IsFixedLength(true);

                entity.Property(e => e.Transfers)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("transfers");

                entity.Property(e => e.Transno).HasMaxLength(50);

                entity.Property(e => e.Valuedate)
                    .HasColumnType("datetime")
                    .HasColumnName("valuedate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Vno)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("vno")
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Customerbalanceold>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CUSTOMERBALANCEOLD");

                entity.Property(e => e.AccD)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AccName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.AccNo)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ActualBalance).HasColumnType("money");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditDate).HasColumnType("datetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.AvailableBalance).HasColumnType("money");

                entity.Property(e => e.Bcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ChequeNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Commission).HasColumnType("money");

                entity.Property(e => e.CustomerBalanceId).HasColumnName("CustomerBalanceID");

                entity.Property(e => e.CustomerNo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Dregard).HasColumnName("dregard");

                entity.Property(e => e.Idno)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("IDNo");

                entity.Property(e => e.ModuleId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ModuleID")
                    .IsFixedLength(true);

                entity.Property(e => e.PayrollNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Period)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransDescription)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TransNo).HasMaxLength(20);

                entity.Property(e => e.TransType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Transfers)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ValueDate).HasColumnType("datetime");

                entity.Property(e => e.Vno)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DApprove1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Approve1");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Rno)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("RNo")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<DApprove2>(entity =>
            {
                entity.HasKey(e => e.Rno);

                entity.ToTable("d_Approve2");

                entity.Property(e => e.Rno)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("RNo")
                    .IsFixedLength(true);

                entity.Property(e => e.Approved).HasDefaultValueSql("(0)");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.GlAcc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("glAcc");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");
            });

            modelBuilder.Entity<DAssignmentVehicle>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_AssignmentVehicle");

                entity.Property(e => e.AccnoV)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.ExpeLedger).HasMaxLength(50);

                entity.Property(e => e.ExpenseAcc).HasMaxLength(50);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("UserID");

                entity.Property(e => e.Vehicle)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DBank>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_BANKS");

                entity.Property(e => e.AccType)
                    .HasMaxLength(50)
                    .HasColumnName("accType");

                entity.Property(e => e.Accno)
                    .HasMaxLength(50)
                    .HasColumnName("ACCNO");

                entity.Property(e => e.Address).HasMaxLength(30);

                entity.Property(e => e.AuditId)
                    .HasMaxLength(10)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.BankAccNo).HasMaxLength(50);

                entity.Property(e => e.BankCode).HasMaxLength(10);

                entity.Property(e => e.BankName).HasMaxLength(50);

                entity.Property(e => e.BranchName).HasMaxLength(50);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Telephone).HasMaxLength(15);
            });

            modelBuilder.Entity<DBonu>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Bonus");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Bank).HasMaxLength(50);

                entity.Property(e => e.Bcode).HasMaxLength(50);

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Enddate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Pby).HasMaxLength(50);

                entity.Property(e => e.Sno).HasMaxLength(50);

                entity.Property(e => e.Startdate).HasColumnType("datetime");
            });

            modelBuilder.Entity<DBonus2>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Bonus2");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Mon1)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon10)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon11)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon12)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon2)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon3)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon4)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon5)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon6)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon7)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon8)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Mon9)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Sno).HasMaxLength(50);
            });

            modelBuilder.Entity<DBranch>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_Branch");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Bname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BName");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LocalId).HasColumnName("local_id");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");
            });
            modelBuilder.Entity<DBankBranch>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_BankBranch");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.BankCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BankCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Bname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BName");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LocalId).HasColumnName("local_id");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DBranchProduct>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_BranchProduct");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Bname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BName");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.LocalId).HasColumnName("local_id");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DBranchsalesman>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_branchsalesman");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Bname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BName");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Idno)
                    .HasMaxLength(50)
                    .HasColumnName("IDNO");

                entity.Property(e => e.LocalId).HasColumnName("local_id");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .HasColumnName("PHONE");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DCashPay>(entity =>
            {
                entity.HasKey(e => e.PayId);

                entity.ToTable("d_CashPay");

                entity.Property(e => e.PayId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Cracc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CRacc");

                entity.Property(e => e.Descr)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Dracc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DRacc");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Payee)
                    .HasMaxLength(85)
                    .IsUnicode(false);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.Vno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("VNo");
            });

            modelBuilder.Entity<DCashShare>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_CashShares");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Period).HasColumnType("datetime");

                entity.Property(e => e.Ref)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("SNo");
            });

            modelBuilder.Entity<DChangepro>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_changepro");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Pprice).HasColumnName("PPrice");

                entity.Property(e => e.Sprice).HasColumnName("SPrice");

                entity.Property(e => e.User)
                    .HasMaxLength(50)
                    .HasColumnName("user");
            });

            modelBuilder.Entity<DCompany>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_company");

                entity.Property(e => e.Acc).HasColumnName("acc");

                entity.Property(e => e.Adress)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.District)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Division)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FaxNo)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Fiscal)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Location)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Motto)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("motto");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Period)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Province)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SendTime)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Smscost)
                    .HasColumnType("money")
                    .HasColumnName("SMSCost");

                entity.Property(e => e.Smsno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SMSNo");

                entity.Property(e => e.Smsport)
                    .HasColumnName("SMSPort")
                    .HasDefaultValueSql("(3)");

                entity.Property(e => e.Town)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Website)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DCostCent>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_CostCent");

                entity.Property(e => e.Comments).HasMaxLength(50);

                entity.Property(e => e.Costcode).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Ttime)
                    .HasColumnType("datetime")
                    .HasColumnName("TTime");

                entity.Property(e => e.Uuser)
                    .HasMaxLength(50)
                    .HasColumnName("UUser");
            });

            modelBuilder.Entity<DCtype>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_CType");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ContCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ContName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DDailySumm>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("d_DailySumm");

                entity.Property(e => e.Ppu)
                    .HasColumnType("money")
                    .HasColumnName("PPU");

                entity.Property(e => e.Qnty)
                    .HasColumnType("money")
                    .HasColumnName("QNTY");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<DDailySummary>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_DailySummary");

                entity.Property(e => e.EndPeriod).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LocalId).HasColumnName("local_id");

                entity.Property(e => e.Location)
                    .HasMaxLength(50)
                    .HasColumnName("LOCATION");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.Total).HasComputedColumnSql("(((((((((((((((((((((((((((((([1]+[2])+[3])+[4])+[5])+[6])+[7])+[8])+[9])+[10])+[11])+[12])+[13])+[14])+[15])+[16])+[17])+[18])+[19])+[20])+[21])+[22])+[23])+[24])+[25])+[26])+[27])+[28])+[29])+[30])+[31])", false);

                entity.Property(e => e._1)
                    .HasColumnName("1")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._10)
                    .HasColumnName("10")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._11)
                    .HasColumnName("11")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._12)
                    .HasColumnName("12")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._13)
                    .HasColumnName("13")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._14)
                    .HasColumnName("14")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._15)
                    .HasColumnName("15")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._16)
                    .HasColumnName("16")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._17)
                    .HasColumnName("17")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._18)
                    .HasColumnName("18")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._19)
                    .HasColumnName("19")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._2)
                    .HasColumnName("2")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._20)
                    .HasColumnName("20")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._21)
                    .HasColumnName("21")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._22)
                    .HasColumnName("22")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._23)
                    .HasColumnName("23")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._24)
                    .HasColumnName("24")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._25)
                    .HasColumnName("25")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._26)
                    .HasColumnName("26")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._27)
                    .HasColumnName("27")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._28)
                    .HasColumnName("28")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._29)
                    .HasColumnName("29")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._3)
                    .HasColumnName("3")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._30)
                    .HasColumnName("30")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._31)
                    .HasColumnName("31")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._4)
                    .HasColumnName("4")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._5)
                    .HasColumnName("5")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._6)
                    .HasColumnName("6")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._7)
                    .HasColumnName("7")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._8)
                    .HasColumnName("8")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e._9)
                    .HasColumnName("9")
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DDailySummaryClerk>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_DailySummaryClerk");

                entity.Property(e => e.EndPeriod).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.Total).HasComputedColumnSql("([1] + [2] + [3] + [4] + [5] + [6] + [7] + [8] + [9] + [10] + [11] + [12] + [13] + [14] + [15] + [16] + [17] + [18] + [19] + [20] + [21] + [22] + [23] + [24] + [25] + [26] + [27] + [28] + [29] + [30] + [31])", false);

                entity.Property(e => e._1)
                    .HasColumnName("1")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._10)
                    .HasColumnName("10")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._11)
                    .HasColumnName("11")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._12)
                    .HasColumnName("12")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._13)
                    .HasColumnName("13")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._14)
                    .HasColumnName("14")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._15)
                    .HasColumnName("15")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._16)
                    .HasColumnName("16")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._17)
                    .HasColumnName("17")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._18)
                    .HasColumnName("18")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._19)
                    .HasColumnName("19")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._2)
                    .HasColumnName("2")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._20)
                    .HasColumnName("20")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._21)
                    .HasColumnName("21")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._22)
                    .HasColumnName("22")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._23)
                    .HasColumnName("23")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._24)
                    .HasColumnName("24")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._25)
                    .HasColumnName("25")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._26)
                    .HasColumnName("26")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._27)
                    .HasColumnName("27")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._28)
                    .HasColumnName("28")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._29)
                    .HasColumnName("29")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._3)
                    .HasColumnName("3")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._30)
                    .HasColumnName("30")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._31)
                    .HasColumnName("31")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._4)
                    .HasColumnName("4")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._5)
                    .HasColumnName("5")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._6)
                    .HasColumnName("6")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._7)
                    .HasColumnName("7")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._8)
                    .HasColumnName("8")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e._9)
                    .HasColumnName("9")
                    .HasDefaultValueSql("(0)");
            });

            modelBuilder.Entity<DDcode>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_DCodes");

                entity.Property(e => e.Dcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Contraacc)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Dedaccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");
            });

            modelBuilder.Entity<DDebtor>(entity =>
            {
                entity.HasKey(e => e.Dcode);

                entity.ToTable("d_Debtors");

                entity.Property(e => e.Dcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DCode");

                entity.Property(e => e.AccCr)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AccDr)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Accno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bbranch)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BBranch");

                entity.Property(e => e.Bcode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Capp).HasColumnName("capp");

                entity.Property(e => e.CertNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Crate).HasColumnName("crate");

                entity.Property(e => e.Crcess)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("crcess");

                entity.Property(e => e.Dname)
                    .HasMaxLength(85)
                    .IsUnicode(false)
                    .HasColumnName("DName");

                entity.Property(e => e.Drcess)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("drcess");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Locations)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phoneno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.Tbranch)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TBranch");

                entity.Property(e => e.Town)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TregDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<DDebtors2>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Debtors2");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.CertNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dcode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dname)
                    .HasMaxLength(85)
                    .IsUnicode(false)
                    .HasColumnName("DName");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Locations)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.TregDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<DDebtorsparchase>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Debtorsparchases");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Debtor).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<DDebtorsparchases2>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Debtorsparchases2");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Balance)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Expenses).HasColumnType("money");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Kgs).HasColumnName("kgs");

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("money")
                    .HasColumnName("Paid Amount");

                entity.Property(e => e.Remarks).HasMaxLength(50);

                entity.Property(e => e.Vehicle).HasMaxLength(50);
            });

            modelBuilder.Entity<DDispatch>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_dispatch");

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Descrip)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("descrip");

                entity.Property(e => e.Dipping)
                    .HasColumnName("dipping")
                    .HasDefaultValueSql("(0.00)");

                entity.Property(e => e.Dispatch)
                    .HasColumnName("dispatch")
                    .HasDefaultValueSql("(0.00)");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Intake).HasDefaultValueSql("(0.00)");

                entity.Property(e => e.Transdate).HasColumnType("datetime");
            });

            modelBuilder.Entity<DDistrict>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Districts");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid")
                    .IsFixedLength(true);

                entity.Property(e => e.Dcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Dname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DName");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DGlposting>(entity =>
            {
                entity.HasKey(e => new { e.Mmonth, e.Yyear });

                entity.ToTable("d_glposting");

                entity.Property(e => e.Mmonth).HasColumnName("mmonth");

                entity.Property(e => e.Yyear).HasColumnName("yyear");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Namount)
                    .HasColumnType("money")
                    .HasColumnName("namount");

                entity.Property(e => e.Posted).HasColumnName("posted");
            });

            modelBuilder.Entity<DHeader>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Headers");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Hcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("HCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Hname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("HName");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DIncomestate>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_incomestate");

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Diff).HasDefaultValueSql("((0))");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Purchases).HasDefaultValueSql("((0))");

                entity.Property(e => e.Sales).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DInvoice>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Invoice");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Desc)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.InvDate).HasColumnType("datetime");

                entity.Property(e => e.InvId)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.Rno)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("RNo");

                entity.Property(e => e.Vendor)
                    .HasMaxLength(35)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DLocation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Location");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Lcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("LCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Lname)
                    .HasMaxLength(85)
                    .IsUnicode(false)
                    .HasColumnName("LName");
            });

            modelBuilder.Entity<DLpo>(entity =>
            {
                entity.HasKey(e => e.Pno);

                entity.ToTable("d_LPO");

                entity.Property(e => e.Pno)
                    .ValueGeneratedNever()
                    .HasColumnName("PNo");

                entity.Property(e => e.Auditdatetieme)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetieme")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.RefNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Remarks)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Serial)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.Vendor)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DMQsetting>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_M_QSettings");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Criteria)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Dvalue)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("dvalue");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Reasons)
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.Property(e => e.RejId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("RejID")
                    .IsFixedLength(true);

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DMainAccount>(entity =>
            {
                entity.HasKey(e => e.Mcode);

                entity.ToTable("d_MainAccount");

                entity.Property(e => e.Mcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Hcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("HCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Mname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MName");
            });

            modelBuilder.Entity<DMaxShare>(entity =>
            {
                entity.HasKey(e => e.IdNo);

                entity.ToTable("d_MaxShares");

                entity.Property(e => e.IdNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AuditId)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.DateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.MaxAmnt).HasColumnType("money");
            });

            modelBuilder.Entity<DMilkBranch>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_MilkBranch");

                entity.Property(e => e.Actual).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Quantity).HasColumnType("money");

                entity.Property(e => e.Variance).HasColumnType("money");

                entity.Property(e => e.Vehicle).HasMaxLength(50);
            });

            modelBuilder.Entity<DMilkControl>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_MilkControl");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreditAcc)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Dcode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DCode")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DebitAcc)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.DispDate).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.PaidAmount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.Vehicleno)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("vehicleno");
            });

            modelBuilder.Entity<DMilkControl1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_MilkControl1");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreditAcc)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dcode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("dcode")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DebitAcc)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DispDate).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DMilkQuality>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_MilkQuality");

                entity.Property(e => e.Alcohol)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Conttype)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Organoleptic)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pcheck).HasColumnName("PCheck");

                entity.Property(e => e.RejDate).HasColumnType("datetime");

                entity.Property(e => e.RejReasons)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TimeIn)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeOut)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransMode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ttransporter)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TTransporter");
            });

            modelBuilder.Entity<DMilkVehicle>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_MilkVehicle");

                entity.Property(e => e.Actual).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Quantity).HasColumnType("money");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .HasColumnName("user")
                    .IsFixedLength(true);

                entity.Property(e => e.Varriance).HasColumnType("money");

                entity.Property(e => e.Vehicle).HasMaxLength(50);
            });

            modelBuilder.Entity<DMilkintake>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_Milkintake");

                entity.HasIndex(e => new { e.Id, e.TransDate, e.Sno }, "IX_d_Milkintake")
                    .IsClustered();

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Comment)
                    .HasMaxLength(50)
                    .HasColumnName("comment");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.LocalId).HasColumnName("local_id");

                entity.Property(e => e.Location)
                    .IsUnicode(false)
                    .HasColumnName("LOCATION");

                entity.Property(e => e.Descript)
                   .HasMaxLength(50)
                   .HasColumnName("Descript");

                entity.Property(e => e.Lr).HasColumnName("LR");

                entity.Property(e => e.Ppu)
                    .HasColumnType("money")
                    .HasColumnName("PPU");

                entity.Property(e => e.Qsupplied)
                    .HasColumnType("money")
                    .HasColumnName("QSupplied");

                entity.Property(e => e.CR)
                    .HasColumnType("money")
                    .HasColumnName("CR");

                entity.Property(e => e.DR)
                   .HasColumnType("money")
                   .HasColumnName("DR");

                entity.Property(e => e.BAL)
                   .HasColumnType("money")
                   .HasColumnName("BAL");

                entity.Property(e => e.Remark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("remark");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.Status1).HasColumnName("status1");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransTime)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<DMilkintake1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Milkintake1");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Comment)
                    .HasMaxLength(50)
                    .HasColumnName("comment");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.LocalId).HasColumnName("local_id");

                entity.Property(e => e.Location)
                    .IsUnicode(false)
                    .HasColumnName("LOCATION");

                entity.Property(e => e.Lr).HasColumnName("LR");

                entity.Property(e => e.Pamount)
                    .HasColumnType("money")
                    .HasColumnName("PAmount");

                entity.Property(e => e.Ppu)
                    .HasColumnType("money")
                    .HasColumnName("PPU");

                entity.Property(e => e.Qsupplied)
                    .HasColumnType("money")
                    .HasColumnName("QSupplied");

                entity.Property(e => e.Remark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("remark");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.Status1).HasColumnName("status1");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransTime)
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DMilkintake2>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Milkintake2");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Comment)
                    .HasMaxLength(50)
                    .HasColumnName("comment");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.LocalId).HasColumnName("local_id");

                entity.Property(e => e.Location)
                    .IsUnicode(false)
                    .HasColumnName("LOCATION");

                entity.Property(e => e.Lr).HasColumnName("LR");

                entity.Property(e => e.Pamount)
                    .HasColumnType("money")
                    .HasColumnName("PAmount");

                entity.Property(e => e.Ppu)
                    .HasColumnType("money")
                    .HasColumnName("PPU");

                entity.Property(e => e.Qsupplied)
                    .HasColumnType("money")
                    .HasColumnName("QSupplied");

                entity.Property(e => e.Remark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("remark");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.Status1).HasColumnName("status1");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransTime)
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DMilkintakeBackup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Milkintake BACKUP");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Location)
                    .IsUnicode(false)
                    .HasColumnName("LOCATION");

                entity.Property(e => e.Lr).HasColumnName("LR");

                entity.Property(e => e.Pamount)
                    .HasColumnType("money")
                    .HasColumnName("PAmount");

                entity.Property(e => e.Ppu)
                    .HasColumnType("money")
                    .HasColumnName("PPU");

                entity.Property(e => e.Qsupplied)
                    .HasColumnType("money")
                    .HasColumnName("QSupplied");

                entity.Property(e => e.Remark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("remark");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransTime)
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DMilkintakechange>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_milkintakechange");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.DateFrom)
                    .HasColumnType("datetime")
                    .HasColumnName("Date From");

                entity.Property(e => e.DateTo)
                    .HasColumnType("datetime")
                    .HasColumnName("Date To");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Userid)
                    .HasMaxLength(50)
                    .HasColumnName("userid");
            });

            modelBuilder.Entity<DMpayement>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Mpayement");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.PhoneNo).HasMaxLength(50);

                entity.Property(e => e.RefNo).HasMaxLength(20);

                entity.Property(e => e.Sno).HasColumnName("SNo");
            });

            modelBuilder.Entity<DOutlet>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Outlet");

                entity.Property(e => e.Branch)
                    .HasMaxLength(30)
                    .IsFixedLength(true);

                entity.Property(e => e.DateEntered)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Date_Entered");

                entity.Property(e => e.OBal).HasColumnName("o_bal");

                entity.Property(e => e.PCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.PName)
                    .HasMaxLength(50)
                    .HasColumnName("p_name");

                entity.Property(e => e.Rprice).HasColumnType("money");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UserId)
                    .HasMaxLength(10)
                    .HasColumnName("user_id")
                    .IsFixedLength(true);

                entity.Property(e => e.Wprice).HasColumnType("money");
            });

            modelBuilder.Entity<DOutletDispatch>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_OutletDispatch");

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.OutletName).HasMaxLength(50);

                entity.Property(e => e.Vehicle)
                    .HasMaxLength(50)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<DOutletSale>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_OutletSales");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.AuditId).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Mpesa).HasMaxLength(50);

                entity.Property(e => e.OutletName).HasMaxLength(50);

                entity.Property(e => e.Paid)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0.0))");

                entity.Property(e => e.Pcode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("PCode");

                entity.Property(e => e.Pname)
                    .HasMaxLength(50)
                    .HasColumnName("PName");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DOutletVehicle>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_OutletVehicle");

                entity.Property(e => e.Customer).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.Vehicle).HasMaxLength(50);
            });

            modelBuilder.Entity<DOutletbranch>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Outletbranch");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bcode1)
                    .HasMaxLength(10)
                    .HasColumnName("BCode1")
                    .IsFixedLength(true);

                entity.Property(e => e.Bname1)
                    .HasMaxLength(50)
                    .HasColumnName("BName1");

                entity.Property(e => e.Cr)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Dr)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");
            });

            modelBuilder.Entity<DOutletstock>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Outletstock");

                entity.Property(e => e.DateEntered)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Date_Entered");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.OutletName).HasMaxLength(50);

                entity.Property(e => e.PName)
                    .HasMaxLength(50)
                    .HasColumnName("p_name");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DOutsale>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Outsales");

                entity.Property(e => e.Accno).HasMaxLength(50);

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DOutsalesb>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Outsalesb");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Apaid)
                    .HasColumnType("money")
                    .HasColumnName("APaid");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Owner).HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DPayment>(entity =>
            {
                entity.HasKey(e => new { e.Rno, e.Vendor })
                    .HasName("PK_d_InvPayment");

                entity.ToTable("d_Payment");

                entity.Property(e => e.Rno)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("RNo");

                entity.Property(e => e.Vendor)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Amnt).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Balance).HasColumnType("money");

                entity.Property(e => e.Category)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Glacc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("GLAcc");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.InvId)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.PayDate).HasColumnType("datetime");

                entity.Property(e => e.PayMode)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.Pono).HasColumnName("PONo");

                entity.Property(e => e.Vno)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("VNo");
            });

            modelBuilder.Entity<DPaymentReq>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_PaymentReq");

                entity.Property(e => e.Amnt).HasColumnType("money");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("auditId");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Desc)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.InvDate).HasColumnType("datetime");

                entity.Property(e => e.InvId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Rno)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("RNo");

                entity.Property(e => e.Status)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('New')");

                entity.Property(e => e.Vendor)
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DPayroll>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_Payroll");

                entity.Property(e => e.AccountNumber).HasMaxLength(255);

                entity.Property(e => e.Advance).HasColumnType("money");

                entity.Property(e => e.Advanceaccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("advanceaccno")
                    .IsFixedLength(true);

                entity.Property(e => e.Agrovet).HasColumnType("money");

                entity.Property(e => e.Agrovetaccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("agrovetaccno")
                    .IsFixedLength(true);

                entity.Property(e => e.Aiaccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("AIaccno")
                    .IsFixedLength(true);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("auditid")
                    .IsFixedLength(true);

                entity.Property(e => e.Bank).HasMaxLength(30);

                entity.Property(e => e.Bbranch)
                    .HasMaxLength(255)
                    .HasColumnName("BBranch");

                entity.Property(e => e.Bonus).HasColumnType("money");

                entity.Property(e => e.Cbo)
                    .HasColumnType("money")
                    .HasColumnName("CBO");

                entity.Property(e => e.Deduct12).HasColumnType("money");

                entity.Property(e => e.EndofPeriod).HasColumnType("datetime");

                entity.Property(e => e.Fsa)
                    .HasColumnType("money")
                    .HasColumnName("FSA");

                entity.Property(e => e.Fsaaccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("fsaaccno")
                    .IsFixedLength(true);

                entity.Property(e => e.Gpay)
                    .HasColumnType("money")
                    .HasColumnName("GPay");

                entity.Property(e => e.Hshares)
                    .HasColumnType("money")
                    .HasColumnName("HShares");

                entity.Property(e => e.Hsharesaccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("hsharesaccno")
                    .IsFixedLength(true);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IdNo).HasMaxLength(50);

                entity.Property(e => e.Mainaccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("mainaccno")
                    .IsFixedLength(true);

                entity.Property(e => e.Midmonth)
                    .HasColumnType("money")
                    .HasColumnName("midmonth");

                entity.Property(e => e.Mmonth)
                    .HasColumnName("MMonth");

                entity.Property(e => e.Mpesa).HasColumnType("money");

                entity.Property(e => e.Netaccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("netaccno")
                    .IsFixedLength(true);

                entity.Property(e => e.Npay)
                    .HasColumnType("money")
                    .HasColumnName("NPay");

                entity.Property(e => e.Otheraccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("otheraccno")
                    .IsFixedLength(true);

                entity.Property(e => e.Others).HasColumnType("money");

                entity.Property(e => e.Sbranch)
                    .HasMaxLength(25)
                    .HasColumnName("SBranch");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.Subsidy)
                    .HasColumnType("money")
                    .HasColumnName("subsidy");

                entity.Property(e => e.Tchp).HasColumnType("money");

                entity.Property(e => e.Tdeductions)
                    .HasColumnType("money")
                    .HasColumnName("TDeductions")
                    .HasComment("Total deductions");

                entity.Property(e => e.Tmshares)
                    .HasColumnType("money")
                    .HasColumnName("TMShares");

                entity.Property(e => e.Tmsharesaccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("tmsharesaccno")
                    .IsFixedLength(true);

                entity.Property(e => e.Trader).HasMaxLength(300);

                entity.Property(e => e.Transport).HasColumnType("money");

                entity.Property(e => e.Transportaccno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("transportaccno")
                    .IsFixedLength(true);

                entity.Property(e => e.Yyear)
                    .HasColumnName("YYear");
            });

            modelBuilder.Entity<DPayrollCopy>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_PayrollCopy");

                entity.Property(e => e.AuditDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DatePosted)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Location).HasMaxLength(50);

                entity.Property(e => e.Names)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NetPay).HasColumnType("money");

                entity.Property(e => e.PhoneNo).HasMaxLength(50);

                entity.Property(e => e.Phoneno2).HasMaxLength(50);

                entity.Property(e => e.Run2).HasDefaultValueSql("((0))");

                entity.Property(e => e.Sno)
                    .HasMaxLength(50)
                    .HasColumnName("SNo");

                entity.Property(e => e.Status1).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status2).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status3).HasDefaultValueSql("((0))");

                entity.Property(e => e.User1)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.User2)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.User3)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DPeriod>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Periods");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditId");

                entity.Property(e => e.AuditdateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdateTime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndPeriod).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DPreSet>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_PreSets");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.BranchCode)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Deduction)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Rate).HasColumnType("money");

                entity.Property(e => e.Rated).HasDefaultValueSql("((1))");

                entity.Property(e => e.Remark)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.StartDate)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Status2)
                    .HasColumnName("status2")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Status3)
                    .HasColumnName("status3")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Status4)
                    .HasColumnName("status4")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Status5)
                    .HasColumnName("status5")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Status6)
                    .HasColumnName("status6")
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DPrice>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_Price");

                entity.Property(e => e.Edate)
                    .HasColumnType("datetime")
                    .HasColumnName("EDate");
                entity.Property(e => e.SaccoCode);
                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Products)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.SubsidyQty);
                entity.Property(e => e.SubsidyPrice).HasColumnType("money");
                entity.Property(e => e.DrAccNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DrAccNo");
                entity.Property(e => e.CrAccNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CrAccNo");
            });

            modelBuilder.Entity<DPrice2>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Price2");

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(85);

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Sno)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<DPriceBranch>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_PriceBranch");

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(85);

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Sno)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<DProductProcess>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_ProductProcess");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Quantity).HasDefaultValueSql("((0))");

                entity.Property(e => e.Remarks).HasMaxLength(50);
            });

            modelBuilder.Entity<DQuality>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Quality");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Canno)
                    .HasMaxLength(50)
                    .HasColumnName("canno");

                entity.Property(e => e.Enddate).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Quality).HasMaxLength(50);

                entity.Property(e => e.Quantity).HasMaxLength(50);

                entity.Property(e => e.Rate).HasMaxLength(50);

                entity.Property(e => e.Sno)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Startdate)
                    .HasColumnType("datetime")
                    .HasColumnName("startdate");
            });

            modelBuilder.Entity<DReceipt>(entity =>
            {
                entity.HasKey(e => e.DelNo);

                entity.ToTable("d_Receipts");

                entity.Property(e => e.DelNo)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Qnty).HasDefaultValueSql("(0.00)");

                entity.Property(e => e.Remarks)
                    .HasMaxLength(85)
                    .IsUnicode(false);

                entity.Property(e => e.Rno)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("RNo");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.Vendor)
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DRegistration>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_Registration");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("amount");

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdate");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bal)
                    .HasColumnType("money")
                    .HasColumnName("bal");

                entity.Property(e => e.Datepostedtoledger)
                    .HasColumnType("datetime")
                    .HasColumnName("datepostedtoledger");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.LocalId).HasColumnName("local_id");

                entity.Property(e => e.Mno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mno");

                entity.Property(e => e.Run)
                    .HasColumnName("RUN")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Sno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("sno");

                entity.Property(e => e.Toledgers).HasColumnName("toledgers");

                entity.Property(e => e.Transdate)
                    .HasColumnType("datetime")
                    .HasColumnName("transdate");

                entity.Property(e => e.Transdescription)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("transdescription");

                entity.Property(e => e.Userledger)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userledger");
            });

            modelBuilder.Entity<DRequisition>(entity =>
            {
                entity.HasKey(e => e.Rno);

                entity.ToTable("d_Requisition");

                entity.Property(e => e.Rno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("RNo");

                entity.Property(e => e.AuditDatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CostCentre)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Iname)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("IName");

                entity.Property(e => e.Make)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Pricing)
                    .HasColumnType("money")
                    .HasColumnName("pricing")
                    .HasDefaultValueSql("(0.00)");

                entity.Property(e => e.Rbatch)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ReqDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ServiceReq).HasDefaultValueSql("(0)");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('New')");

                entity.Property(e => e.TransDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<DSconribution>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_sconribution");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("amount");

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdate");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bal)
                    .HasColumnType("money")
                    .HasColumnName("bal");

                entity.Property(e => e.Datepostedtoledger)
                    .HasColumnType("datetime")
                    .HasColumnName("datepostedtoledger")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Mno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mno");

                entity.Property(e => e.Remarks).HasMaxLength(50);

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Toledgers)
                    .HasColumnName("toledgers")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Transdate)
                    .HasColumnType("datetime")
                    .HasColumnName("transdate");

                entity.Property(e => e.Transdescription)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("transdescription");

                entity.Property(e => e.Userledger)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userledger");
            });

            modelBuilder.Entity<DShare>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Shares");

                entity.Property(e => e.Amnt).HasColumnType("money");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("amount");

                entity.Property(e => e.AuditDateTime).HasColumnType("datetime");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.Bal)
                    .HasColumnType("money")
                    .HasColumnName("bal");

                entity.Property(e => e.Code)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IdNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Loc)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Mno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.Property(e => e.Period).IsUnicode(false);

                entity.Property(e => e.Pmode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("pmode");

                entity.Property(e => e.Premium)
                    .HasColumnType("money")
                    .HasColumnName("PREMIUM")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Regdate).HasColumnType("datetime");

                entity.Property(e => e.Sex)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Shares).HasColumnType("money");

                entity.Property(e => e.Sno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("sno");

                entity.Property(e => e.Spu)
                    .HasColumnType("money")
                    .HasColumnName("spu")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DSmscompany>(entity =>
            {
                entity.ToTable("d_SMSCompany");

                entity.Property(e => e.DateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Message)
                    .HasMaxLength(160)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(13)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DSmssetting>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_smssettings");

                entity.Property(e => e.Balanace).HasColumnType("money");

                entity.Property(e => e.Cr).HasColumnType("money");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Dr).HasColumnType("money");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");
            });

            modelBuilder.Entity<DSupplier>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_Suppliers");

                entity.Property(e => e.Sno)
                    .ValueGeneratedNever()
                    .HasColumnName("SNo");

                entity.Property(e => e.Aarno)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("aarno");

                entity.Property(e => e.AccNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Bbranch)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BBranch");

                entity.Property(e => e.Bcode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Br)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValueSql("(N'A')")
                    .IsFixedLength(true);

                entity.Property(e => e.Branch)
                    .HasMaxLength(85)
                    .IsUnicode(false);

                entity.Property(e => e.Branchcode).HasColumnName("branchcode");

                entity.Property(e => e.Compare)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.District)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Division)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dob)
                    .HasColumnType("datetime")
                    .HasColumnName("dob");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Frate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("frate");

                entity.Property(e => e.Freezed)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("freezed")
                    .HasDefaultValueSql("((0))")
                    .IsFixedLength(true);

                entity.Property(e => e.HasNursery).HasMaxLength(50);

                entity.Property(e => e.Hast).HasColumnName("hast");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IdNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Isfrate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("isfrate");

                entity.Property(e => e.LocalId).HasColumnName("LOCAL_ID");

                entity.Property(e => e.Location)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mass)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("mass")
                    .HasDefaultValueSql("((0))")
                    .IsFixedLength(true);

                entity.Property(e => e.Mno)
                    .HasMaxLength(10)
                    .HasColumnName("mno")
                    .IsFixedLength(true);

                entity.Property(e => e.Names)
                    .HasMaxLength(85)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Photo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("photo");

                entity.Property(e => e.Rate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("rate")
                    .IsFixedLength(true);

                entity.Property(e => e.Regdate).HasColumnType("datetime");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.Scode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("scode")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Sign)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("sign");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.Status1).HasColumnName("status1");

                entity.Property(e => e.Status2).HasColumnName("status2");

                entity.Property(e => e.Status3).HasColumnName("status3");

                entity.Property(e => e.Status4).HasColumnName("status4");

                entity.Property(e => e.Status5).HasColumnName("status5");

                entity.Property(e => e.Status6)
                    .HasMaxLength(50)
                    .HasColumnName("status6");

                entity.Property(e => e.Thcpactive).HasColumnName("thcpactive");

                entity.Property(e => e.Thcppremium).HasColumnName("thcppremium");

                entity.Property(e => e.Tmd)
                    .HasColumnType("datetime")
                    .HasColumnName("tmd");

                entity.Property(e => e.Town)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Trader).HasDefaultValueSql("((0))");

                entity.Property(e => e.TransCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("transporters code");

                entity.Property(e => e.Type)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasComment("eg male female orga");

                entity.Property(e => e.Types)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("types");

                entity.Property(e => e.Village)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DSupplierDeduc>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_supplier_deduc");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Branch)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Branchcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("branchcode")
                    .IsFixedLength(true);

                entity.Property(e => e.DateDeduc)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Deduc");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Period)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Remarks)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status1).HasColumnName("status1");

                entity.Property(e => e.Status2).HasColumnName("status2");

                entity.Property(e => e.Status3).HasColumnName("status3");

                entity.Property(e => e.Status4).HasColumnName("status4");

                entity.Property(e => e.Status5).HasColumnName("status5");

                entity.Property(e => e.Status6).HasColumnName("status6");

                entity.Property(e => e.Yyear).HasColumnName("yyear");
            });

            modelBuilder.Entity<DSupplierStandingorder>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_supplier_standingorder");

                entity.Property(e => e.Active).HasDefaultValueSql("((0))");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Complete)
                    .HasColumnName("complete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DateDeduc)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Deduc");

                entity.Property(e => e.DateStop)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_stop");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.MaxAmount).HasColumnType("money");

                entity.Property(e => e.Period)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Remarks)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("SNO");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Topup)
                    .HasColumnName("topup")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Yyear).HasColumnName("yyear");
            });

            modelBuilder.Entity<DSupplyDeducTran>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_SupplyDeducTrans");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("auditid")
                    .IsFixedLength(true);

                entity.Property(e => e.Balance).HasColumnType("money");

                entity.Property(e => e.DedCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Isfrate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("isfrate");

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.TranType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Yyear)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("yyear")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<DTblTrend>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_tbl_trends");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Average)
                    .HasColumnType("money")
                    .HasColumnName("average");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Kgs)
                    .HasColumnType("money")
                    .HasColumnName("kgs");

                entity.Property(e => e.Sno)
                    .HasColumnName("sno")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Transdate)
                    .HasColumnType("datetime")
                    .HasColumnName("transdate");
            });

            modelBuilder.Entity<DTmpEnquery>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_tmpEnquery");

                entity.Property(e => e.Bal).HasColumnType("money");

                entity.Property(e => e.Cr)
                    .HasColumnType("money")
                    .HasColumnName("CR");

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Dr)
                    .HasColumnType("money")
                    .HasColumnName("DR");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.Type2).HasMaxLength(50);
            });

            modelBuilder.Entity<DTmpTransEnquery>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_tmpTransEnquery");

                entity.Property(e => e.Bal).HasColumnType("money");

                entity.Property(e => e.Code)
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.Cr)
                    .HasColumnType("money")
                    .HasColumnName("CR");

                entity.Property(e => e.Dr)
                    .HasColumnType("money")
                    .HasColumnName("DR");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Sno)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("SNo");

                entity.Property(e => e.TransDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<DTransDetailed>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_TransDetailed");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.EndPeriod).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Mmonth)
                    .HasColumnName("MMonth")
                    .HasComputedColumnSql("(datepart(month,[EndPeriod]))", false);

                entity.Property(e => e.Qnty).HasColumnName("QNTY");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.Subsidy).HasColumnType("money");

                entity.Property(e => e.TransCode)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .HasColumnName("Trans_Code");

                entity.Property(e => e.Yyear)
                    .HasColumnName("YYear")
                    .HasComputedColumnSql("(datepart(year,[EndPeriod]))", false);
            });

            modelBuilder.Entity<DTransFrate>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_trans_frate");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("(0.00)");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Days).HasColumnName("days");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Isfrate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("isfrate");

                entity.Property(e => e.Period).HasColumnType("datetime");

                entity.Property(e => e.Rate)
                    .HasColumnName("rate")
                    .HasDefaultValueSql("(0.00)");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.TransCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Trans_code")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<DTransMode>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_TransMode");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Rate).HasColumnName("RATE");

                entity.Property(e => e.TransCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Transport)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DTransport>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_Transport");

                entity.Property(e => e.Active).HasDefaultValueSql("(1)");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.DateInactivate).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Isfrate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("isfrate");

                entity.Property(e => e.Rate).HasColumnType("money");

                entity.Property(e => e.Startdate)
                    .HasColumnType("datetime")
                    .HasColumnName("startdate");

                entity.Property(e => e.TransCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Trans_Code")
                    .IsFixedLength(true); 
            });

            modelBuilder.Entity<DTransportDeduc>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_Transport_Deduc");

                entity.Property(e => e.Ai).HasColumnName("AI");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasComment("serves as deduction code");

                entity.Property(e => e.Enddate)
                    .HasColumnType("datetime")
                    .HasColumnName("enddate");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Period).HasMaxLength(25);

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.Remarks).HasMaxLength(50);

                entity.Property(e => e.Startdate)
                    .HasColumnType("datetime")
                    .HasColumnName("startdate");

                entity.Property(e => e.TdateDeduc)
                    .HasColumnType("datetime")
                    .HasColumnName("TDate_Deduc");

                entity.Property(e => e.TransCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Yyear).HasColumnName("yyear");
            });

            modelBuilder.Entity<DTransportStandingorder>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_transport_standingorder");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.DateDeduc)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Deduc");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.MaxAmount).HasColumnType("money");

                entity.Property(e => e.Period)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Remarks)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.TransCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Yyear).HasColumnName("yyear");
            });

            modelBuilder.Entity<DTransporter>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("d_Transporters");

                entity.Property(e => e.TransCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Accno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdatetime)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bbranch)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BBranch");

                entity.Property(e => e.Bcode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Br)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("BR")
                    .HasDefaultValueSql("(N'A')")
                    .IsFixedLength(true);

                entity.Property(e => e.Canno)
                    .HasMaxLength(50)
                    .HasColumnName("canno");

                entity.Property(e => e.CertNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Freezed)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("freezed")
                    .HasDefaultValueSql("((0))")
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Isfrate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("isfrate");

                entity.Property(e => e.Locations)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ParentT)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phoneno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.Tbranch)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TBranch");

                entity.Property(e => e.Town)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransName)
                    .HasMaxLength(85)
                    .IsUnicode(false);

                entity.Property(e => e.TregDate).HasColumnType("datetime");

                entity.Property(e => e.Tt).HasColumnName("tt");

                entity.Property(e => e.Ttrate).HasColumnName("ttrate");
            });

            modelBuilder.Entity<DTransportersPayRoll>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_TransportersPayRoll");

                entity.Property(e => e.AccNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Advance).HasColumnType("money");

                entity.Property(e => e.Agrovet).HasColumnType("money");

                entity.Property(e => e.Ai)
                    .HasColumnType("money")
                    .HasColumnName("AI");

                entity.Property(e => e.Amnt).HasColumnType("money");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.BankName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Branch)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndPeriod).HasColumnType("datetime");

                entity.Property(e => e.Frate).HasColumnName("frate");

                entity.Property(e => e.Fsa)
                    .HasColumnType("money")
                    .HasColumnName("FSA");

                entity.Property(e => e.GrossPay).HasColumnType("money");

                entity.Property(e => e.Hshares)
                    .HasColumnType("money")
                    .HasColumnName("HShares");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Isfrate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("isfrate");

                entity.Property(e => e.Mmonth)
                    .HasColumnName("MMonth")
                    .HasComputedColumnSql("(datepart(month,[EndPeriod]))", false);

                entity.Property(e => e.NetPay).HasColumnType("money");

                entity.Property(e => e.Others).HasColumnType("money");

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.Subsidy).HasColumnType("money");

                entity.Property(e => e.Tmshares)
                    .HasColumnType("money")
                    .HasColumnName("TMShares");

                entity.Property(e => e.Totaldeductions).HasColumnType("money");

                entity.Property(e => e.Yyear)
                    .HasColumnName("YYear")
                    .HasComputedColumnSql("(datepart(year,[endperiod]))", false);
            });

            modelBuilder.Entity<DType>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_Type");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BCode")
                    .IsFixedLength(true);

                entity.Property(e => e.Bname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BName");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DVehicleTill>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("d_VehicleTill");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Cr)
                    .HasMaxLength(20)
                    .IsFixedLength(true);

                entity.Property(e => e.Dr)
                    .HasMaxLength(20)
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.TillNo).HasMaxLength(50);

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("UserID");

                entity.Property(e => e.Vehicle).HasMaxLength(50);
            });

            modelBuilder.Entity<DailySupply>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DailySupply");

                entity.Property(e => e.Branch).HasMaxLength(255);

                entity.Property(e => e.Date).HasColumnType("smalldatetime");
            });

            modelBuilder.Entity<Damagedstock>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DAMAGEDSTOCK");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("DATE");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.Month)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("MONTH");

                entity.Property(e => e.Pocode).HasColumnName("POCode");

                entity.Property(e => e.Priceeach)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRICEEACH");

                entity.Property(e => e.Productid)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRODUCTID");

                entity.Property(e => e.Productname)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PRODUCTNAME");

                entity.Property(e => e.Quantity)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.Totalamount)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("TOTALAMOUNT");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("USERNAME");

                entity.Property(e => e.Year)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("YEAR");
            });

            modelBuilder.Entity<DeductionsQuery>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Deductions Query");

                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(255)
                    .HasColumnName("Account Number");

                entity.Property(e => e.Advance).HasColumnType("money");

                entity.Property(e => e.Agrovet).HasColumnType("money");

                entity.Property(e => e.Ai)
                    .HasColumnType("money")
                    .HasColumnName("AI");

                entity.Property(e => e.Branch).HasMaxLength(255);

                entity.Property(e => e.Fsa)
                    .HasColumnType("money")
                    .HasColumnName("FSA");

                entity.Property(e => e.GrossPay)
                    .HasColumnType("money")
                    .HasColumnName("Gross Pay");

                entity.Property(e => e.HShares)
                    .HasColumnType("money")
                    .HasColumnName("H Shares");

                entity.Property(e => e.Mode).HasMaxLength(30);

                entity.Property(e => e.NetPay)
                    .HasColumnType("money")
                    .HasColumnName("Net Pay");

                entity.Property(e => e.Others).HasColumnType("money");

                entity.Property(e => e.Period).HasMaxLength(20);

                entity.Property(e => e.SupplierBranch).HasMaxLength(25);

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(45)
                    .HasColumnName("Supplier Name");

                entity.Property(e => e.SupplierNumber).HasColumnName("Supplier Number");

                entity.Property(e => e.TMShares)
                    .HasColumnType("money")
                    .HasColumnName("T M Shares");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.TotalKgs).HasColumnName("TOTAL(Kgs)");

                entity.Property(e => e.Trader).HasMaxLength(3);

                entity.Property(e => e.Transport).HasColumnType("money");

                entity.Property(e => e.Type).HasMaxLength(255);
            });

            modelBuilder.Entity<Depreciation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Depreciation");

                entity.Property(e => e.Assetcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("assetcode");

                entity.Property(e => e.DepreciationAmt).HasColumnType("money");

                entity.Property(e => e.Mmonth).HasColumnName("mmonth");

                entity.Property(e => e.Uuser)
                    .HasMaxLength(50)
                    .HasColumnName("uuser");

                entity.Property(e => e.Yyear).HasColumnName("yyear");
            });

            modelBuilder.Entity<DetailedTransport>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DetailedTransport");

                entity.Property(e => e.Branch).HasMaxLength(255);

                entity.Property(e => e.Code).HasMaxLength(255);

                entity.Property(e => e.Period).HasMaxLength(25);

                entity.Property(e => e.SupplierNumber).HasColumnName("Supplier Number");
            });

            modelBuilder.Entity<DisTmpActiveSup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("dis_tmpActiveSup");

                entity.Property(e => e.Active).HasDefaultValueSql("(0)");

                entity.Property(e => e.Suppliers).HasDefaultValueSql("(0)");

                entity.Property(e => e.Type)
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Drange>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DRanges");

                entity.Property(e => e.Auditid)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("auditid")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Audittime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Dcode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DCode")
                    .IsFixedLength(true);

                entity.Property(e => e.From).HasColumnType("money");

                entity.Property(e => e.Rate)
                    .HasColumnType("money")
                    .HasColumnName("rate");

                entity.Property(e => e.To).HasColumnType("money");
            });

            modelBuilder.Entity<Drawnstock>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DRAWNSTOCK");

                entity.Property(e => e.Ai).HasColumnName("AI");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Buying).HasMaxLength(255);

                entity.Property(e => e.Commission)
                    .HasMaxLength(50)
                    .HasColumnName("commission");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("DATE");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Month)
                    .HasMaxLength(255)
                    .HasColumnName("MONTH");

                entity.Property(e => e.Priceeach)
                    .HasMaxLength(255)
                    .HasColumnName("PRICEEACH");

                entity.Property(e => e.Productid)
                    .HasMaxLength(255)
                    .HasColumnName("PRODUCTID");

                entity.Property(e => e.Productname)
                    .HasMaxLength(255)
                    .HasColumnName("PRODUCTNAME");

                entity.Property(e => e.Quantity)
                    .HasMaxLength(255)
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.Totalamount)
                    .HasMaxLength(255)
                    .HasColumnName("TOTALAMOUNT");

                entity.Property(e => e.Updated).HasColumnName("updated");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .HasColumnName("USERNAME");

                entity.Property(e => e.Year)
                    .HasMaxLength(255)
                    .HasColumnName("YEAR");
            });

            modelBuilder.Entity<EasyMaPolicy>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EasyMa_policies");

                entity.Property(e => e.EnforcePassHistory)
                    .HasColumnName("enforcePassHistory")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Ephnum).HasColumnName("ephnum");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Minpexpire).HasColumnName("minpexpire");

                entity.Property(e => e.PassExpire).HasDefaultValueSql("((30))");

                entity.Property(e => e.PassLength)
                    .HasColumnName("passLength")
                    .HasDefaultValueSql("((5))");

                entity.Property(e => e.Pcomplexity)
                    .HasColumnName("pcomplexity")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Spreverse).HasColumnName("spreverse");
            });

            modelBuilder.Entity<Gledger>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("GLedgers");

                entity.Property(e => e.AccBal).HasColumnType("money");

                entity.Property(e => e.Auditid).HasMaxLength(50);

                entity.Property(e => e.Chequeno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Credits).HasColumnType("money");

                entity.Property(e => e.Debits).HasColumnType("money");

                entity.Property(e => e.Description)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Glname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("GLname");

                entity.Property(e => e.Source)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Transdate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Glsetup>(entity =>
            {
                entity.HasKey(e => new { e.AccNo, e.Glid });

                entity.ToTable("GLSETUP");

                entity.Property(e => e.AccNo).HasMaxLength(50);

                entity.Property(e => e.Glid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("GLid");

                entity.Property(e => e.AccCategory)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Actuals).HasColumnType("money");

                entity.Property(e => e.AuditDate).HasColumnType("datetime");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("AuditID")
                    .IsFixedLength(true);

                entity.Property(e => e.AuditOrg)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Bal).HasColumnType("money");

                entity.Property(e => e.Border).HasColumnName("border");

                entity.Property(e => e.Branch)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Budgetted).HasColumnType("money");

                entity.Property(e => e.Curr)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.CurrCode)
                    .HasColumnType("money")
                    .HasColumnName("Curr_Code");

                entity.Property(e => e.CurrentBal).HasColumnType("money");

                entity.Property(e => e.GlAccGroup)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GlAccMainGroup)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.GlAccName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GlAccStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GlAccType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GlCode).HasMaxLength(50);

                entity.Property(e => e.Hcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Header)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Hname)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IsRearning).HasColumnName("isRearning");

                entity.Property(e => e.Mcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Mheader)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NewGlopeningBal)
                    .HasColumnType("money")
                    .HasColumnName("NewGLOpeningBal");

                entity.Property(e => e.NewGlopeningBalDate)
                    .HasColumnType("datetime")
                    .HasColumnName("NewGLOpeningBalDate");

                entity.Property(e => e.NormalBal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OpeningBal).HasColumnType("money");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.Subtype)
                    .HasMaxLength(50)
                    .HasColumnName("subtype");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<Gltransaction>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("GLTRANSACTIONS");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ChequeNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CrAccNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DocPosted).HasColumnName("doc_posted");

                entity.Property(e => e.DocumentNo).HasMaxLength(200);

                entity.Property(e => e.DrAccNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Dregard)
                    .HasColumnName("dregard")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.LocalId).HasColumnName("LOCAL_ID");

                entity.Property(e => e.Module)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("module");

                entity.Property(e => e.Pmode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pmode");

                entity.Property(e => e.Recon).HasColumnName("RECON");

                entity.Property(e => e.ReconId).HasColumnName("ReconID");

                entity.Property(e => e.Refid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("refid");

                entity.Property(e => e.Run).HasDefaultValueSql("((0))");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TimeTrans)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransDescript)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Transactionno)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Gltransactions2>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("GLTRANSACTIONS2");

                entity.Property(e => e.Accname)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Accno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(100)
                    .HasColumnName("AuditID");

                entity.Property(e => e.Available).HasColumnType("money");

                entity.Property(e => e.ChequeNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Transdate).HasColumnType("datetime");

                entity.Property(e => e.Transdescription)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Transtype)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Journal>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Accno)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("ACCNO");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("AMOUNT");

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasColumnName("AUDITDATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("AUDITID");

                entity.Property(e => e.Jvid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("JVID");

                entity.Property(e => e.Loanno)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Memberno)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("MEMBERNO")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Naration)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("NARATION");

                entity.Property(e => e.Posted).HasColumnName("POSTED");

                entity.Property(e => e.Posteddate)
                    .HasColumnType("datetime")
                    .HasColumnName("POSTEDDATE");

                entity.Property(e => e.Sharetype)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SHARETYPE")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Transdate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRANSDATE");

                entity.Property(e => e.Transtype)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("TRANSTYPE")
                    .IsFixedLength(true);

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TYPE");

                entity.Property(e => e.Vno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("VNO");
            });

            modelBuilder.Entity<JournalType>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Jid)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("JID");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Journalslisting>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Journalslisting");

                entity.Property(e => e.Accno)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("ACCNO");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("AMOUNT");

                entity.Property(e => e.AmountCr)
                    .HasColumnType("money")
                    .HasColumnName("AMOUNT_CR");

                entity.Property(e => e.AmountDr)
                    .HasColumnType("money")
                    .HasColumnName("AMOUNT_DR");

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasColumnName("AUDITDATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("AUDITID");

                entity.Property(e => e.Jvid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("JVID");

                entity.Property(e => e.Loanno)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Memberno)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("MEMBERNO")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Naration)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("NARATION");

                entity.Property(e => e.Posted).HasColumnName("POSTED");

                entity.Property(e => e.Posteddate)
                    .HasColumnType("datetime")
                    .HasColumnName("POSTEDDATE");

                entity.Property(e => e.Sharetype)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SHARETYPE")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Transdate)
                    .HasColumnType("datetime")
                    .HasColumnName("TRANSDATE");

                entity.Property(e => e.Transtype)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("TRANSTYPE")
                    .IsFixedLength(true);

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TYPE");

                entity.Property(e => e.Vno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("VNO");
            });

            modelBuilder.Entity<Kin>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("KIN");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.AuditId)
                    .HasMaxLength(10)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime).HasColumnType("smalldatetime");

                entity.Property(e => e.Comments).HasMaxLength(100);

                entity.Property(e => e.HomeTelNo).HasMaxLength(15);

                entity.Property(e => e.Idno)
                    .HasMaxLength(10)
                    .HasColumnName("IDNo");

                entity.Property(e => e.KinNames).HasMaxLength(50);

                entity.Property(e => e.KinNo).HasMaxLength(50);

                entity.Property(e => e.KinSigned).HasMaxLength(3);

                entity.Property(e => e.MemberNo).HasMaxLength(20);

                entity.Property(e => e.OfficeTelNo).HasMaxLength(15);

                entity.Property(e => e.Relationship).HasMaxLength(15);

                entity.Property(e => e.SignDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Witness).HasColumnType("text");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Location1)
                    .HasMaxLength(255)
                    .HasColumnName("Location");

                entity.Property(e => e.Period).HasMaxLength(25);
            });

            modelBuilder.Entity<Login>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Login");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Level).HasMaxLength(20);

                entity.Property(e => e.OtherNames).HasMaxLength(30);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Surname).HasMaxLength(16);

                entity.Property(e => e.UserName).HasMaxLength(20);
            });

            modelBuilder.Entity<Login1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("LOGINS");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.LogedOut)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'No')");

                entity.Property(e => e.LogoutTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Ttime)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("ttime")
                    .HasDefaultValueSql("(getdate())")
                    .IsFixedLength(true);

                entity.Property(e => e.UserLoginIds)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UserLoginIDs")
                    .IsFixedLength(true);

                entity.Property(e => e.WkStation)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('No WkStation')");
            });

            modelBuilder.Entity<Matchedreport>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Matchedreport");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("AMOUNT");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.BfubId).HasColumnName("BFUB_ID");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.Details).HasMaxLength(250);

                entity.Property(e => e.ExId).HasColumnName("EX_ID");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Names)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("NAMES");

                entity.Property(e => e.Narration)
                    .HasMaxLength(250)
                    .HasColumnName("NARRATION");

                entity.Property(e => e.Phoneno)
                    .HasMaxLength(250)
                    .HasColumnName("phoneno");

                entity.Property(e => e.Sno)
                    .HasMaxLength(50)
                    .HasColumnName("SNO");

                entity.Property(e => e.Valuedate)
                    .HasColumnType("datetime")
                    .HasColumnName("valuedate");
            });

            modelBuilder.Entity<MaxShare>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id)
                    .HasMaxLength(255)
                    .HasColumnName("ID");

                entity.Property(e => e.MaxAmount).HasColumnType("money");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Company')");

                entity.Property(e => e.Content)
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.DateReceived)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.MsgType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessTime)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Replied).HasDefaultValueSql("(0)");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Self')");

                entity.Property(e => e.Telephone)
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Mpesab>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Mpesab");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Balance).HasColumnType("money");

                entity.Property(e => e.CompletionTime).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.InitiationTime).HasColumnType("datetime");

                entity.Property(e => e.LinkedTransactionId).HasColumnName("LinkedTransactionID");

                entity.Property(e => e.Narration).HasColumnName("NARRATION");

                entity.Property(e => e.PaidIn).HasColumnType("money");

                entity.Property(e => e.Phoneno)
                    .HasMaxLength(50)
                    .HasColumnName("phoneno");

                entity.Property(e => e.Reference).HasColumnName("REFERENCE");

                entity.Property(e => e.Run).HasColumnName("RUN");

                entity.Property(e => e.Run2)
                    .HasColumnName("run2")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Withdrawn).HasColumnType("money");
            });

            modelBuilder.Entity<Param>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("param");

                entity.Property(e => e.ADep)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("A_Dep");

                entity.Property(e => e.Acontrol)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Apcontrol)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("APcontrol");

                entity.Property(e => e.Arcontrol)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ARcontrol");

                entity.Property(e => e.GenerateReceiptno)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Yes')")
                    .IsFixedLength(true);

                entity.Property(e => e.PDep)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("P_dep");

                entity.Property(e => e.Paramid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("paramid");

                entity.Property(e => e.REarnings)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("R_Earnings");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.Property(e => e.VatC)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Vat_C");
            });

            modelBuilder.Entity<Passwordhistory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("passwordhistory");

                entity.Property(e => e.Ephnum).HasColumnName("ephnum");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userID");
            });

            modelBuilder.Entity<PaymentBooking>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PaymentBooking");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Audittime)
                    .HasColumnType("datetime")
                    .HasColumnName("audittime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Ccode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Chequeno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Datedeposited)
                    .HasColumnType("datetime")
                    .HasColumnName("datedeposited");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Memberno)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Particulars)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.PayeeDesc)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Posted)
                    .IsRequired()
                    .HasColumnName("posted")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Ptype)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Transactionno)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Transdate).HasColumnType("datetime");

                entity.Property(e => e.VoucherNo)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Payroll>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PAYROLL$");

                entity.Property(e => e.AccountNumber).HasMaxLength(255);

                entity.Property(e => e.Advance).HasMaxLength(255);

                entity.Property(e => e.Advanceaccno)
                    .HasMaxLength(255)
                    .HasColumnName("advanceaccno");

                entity.Property(e => e.Agrovet).HasMaxLength(255);

                entity.Property(e => e.Agrovetaccno)
                    .HasMaxLength(255)
                    .HasColumnName("agrovetaccno");

                entity.Property(e => e.Ai)
                    .HasMaxLength(255)
                    .HasColumnName("AI");

                entity.Property(e => e.Aiaccno)
                    .HasMaxLength(255)
                    .HasColumnName("AIaccno");

                entity.Property(e => e.Auditdatetime)
                    .HasMaxLength(255)
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(255)
                    .HasColumnName("auditid");

                entity.Property(e => e.Bank).HasMaxLength(255);

                entity.Property(e => e.Bbranch)
                    .HasMaxLength(255)
                    .HasColumnName("BBranch");

                entity.Property(e => e.EndofPeriod).HasMaxLength(255);

                entity.Property(e => e.Fsa)
                    .HasMaxLength(255)
                    .HasColumnName("FSA");

                entity.Property(e => e.Fsaaccno)
                    .HasMaxLength(255)
                    .HasColumnName("fsaaccno");

                entity.Property(e => e.Gpay)
                    .HasMaxLength(255)
                    .HasColumnName("GPay");

                entity.Property(e => e.Hshares)
                    .HasMaxLength(255)
                    .HasColumnName("HShares");

                entity.Property(e => e.Hsharesaccno)
                    .HasMaxLength(255)
                    .HasColumnName("hsharesaccno");

                entity.Property(e => e.KgsSupplied).HasMaxLength(255);

                entity.Property(e => e.Mainaccno)
                    .HasMaxLength(255)
                    .HasColumnName("mainaccno");

                entity.Property(e => e.Mmonth)
                    .HasMaxLength(255)
                    .HasColumnName("MMonth");

                entity.Property(e => e.Netaccno)
                    .HasMaxLength(255)
                    .HasColumnName("netaccno");

                entity.Property(e => e.Npay)
                    .HasMaxLength(255)
                    .HasColumnName("NPay");

                entity.Property(e => e.Otheraccno)
                    .HasMaxLength(255)
                    .HasColumnName("otheraccno");

                entity.Property(e => e.Others).HasMaxLength(255);

                entity.Property(e => e.Sbranch)
                    .HasMaxLength(255)
                    .HasColumnName("SBranch");

                entity.Property(e => e.Sno)
                    .HasMaxLength(255)
                    .HasColumnName("SNo");

                entity.Property(e => e.Tdeductions)
                    .HasMaxLength(255)
                    .HasColumnName("TDeductions");

                entity.Property(e => e.Tmshares)
                    .HasMaxLength(255)
                    .HasColumnName("TMShares");

                entity.Property(e => e.Tmsharesaccno)
                    .HasMaxLength(255)
                    .HasColumnName("tmsharesaccno");

                entity.Property(e => e.Trader).HasMaxLength(255);

                entity.Property(e => e.Transport).HasMaxLength(255);

                entity.Property(e => e.Transportaccno)
                    .HasMaxLength(255)
                    .HasColumnName("transportaccno");

                entity.Property(e => e.Yyear)
                    .HasMaxLength(255)
                    .HasColumnName("YYear");
            });

            modelBuilder.Entity<Period>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PERIODS");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Period1).HasColumnName("Period");

                entity.Property(e => e.StartDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<PettyCash>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PettyCash");

                entity.Property(e => e.AccName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Naration)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pvcno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Transdate)
                    .HasColumnType("datetime")
                    .HasColumnName("transdate");
            });

            modelBuilder.Entity<Qbmp>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("QBMPS");

                entity.Property(e => e.Alc)
                    .HasMaxLength(10)
                    .HasColumnName("ALC")
                    .IsFixedLength(true);

                entity.Property(e => e.Anr)
                    .HasMaxLength(10)
                    .HasColumnName("ANR")
                    .IsFixedLength(true);

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Canno)
                    .HasMaxLength(50)
                    .HasColumnName("canno");

                entity.Property(e => e.Cname).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Pscore)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Remarks).HasMaxLength(50);

                entity.Property(e => e.Tpc)
                    .HasMaxLength(10)
                    .HasColumnName("TPC")
                    .IsFixedLength(true);

                entity.Property(e => e.Tsc)
                    .HasMaxLength(10)
                    .HasColumnName("TSC")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Qsetup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Qsetup");

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .HasColumnName("auditid");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Irate)
                    .HasMaxLength(10)
                    .HasColumnName("irate")
                    .IsFixedLength(true);

                entity.Property(e => e.Quality).HasMaxLength(50);
            });

            modelBuilder.Entity<Query1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Query1");

                entity.Property(e => e.AmountSupplied).HasColumnName("Amount Supplied");

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(255)
                    .HasColumnName("Supplier Name");

                entity.Property(e => e.SupplierNumber).HasColumnName("Supplier Number");
            });

            modelBuilder.Entity<QueryBankSalary>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("QueryBankSalary");

                entity.Property(e => e.AccountNumber).HasMaxLength(25);

                entity.Property(e => e.BankName).HasMaxLength(40);

                entity.Property(e => e.Branch).HasMaxLength(40);

                entity.Property(e => e.EmployeeNumber).HasMaxLength(255);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.Property(e => e.NetSalary)
                    .HasColumnType("money")
                    .HasColumnName("Net Salary");

                entity.Property(e => e.Period).HasMaxLength(20);
            });

            modelBuilder.Entity<QueryPayroll>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("QueryPayroll");

                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(35)
                    .HasColumnName("Account Number");

                entity.Property(e => e.Advance).HasColumnType("money");

                entity.Property(e => e.Agrovet).HasColumnType("money");

                entity.Property(e => e.Ai)
                    .HasColumnType("money")
                    .HasColumnName("AI");

                entity.Property(e => e.BankName)
                    .HasMaxLength(45)
                    .HasColumnName("Bank Name");

                entity.Property(e => e.Branch).HasMaxLength(35);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("First Name");

                entity.Property(e => e.GrossPay)
                    .HasColumnType("money")
                    .HasColumnName("Gross Pay");

                entity.Property(e => e.HShares)
                    .HasColumnType("money")
                    .HasColumnName("H Shares");

                entity.Property(e => e.IdNumber)
                    .HasMaxLength(25)
                    .HasColumnName("Id Number");

                entity.Property(e => e.NetPay)
                    .HasColumnType("money")
                    .HasColumnName("Net Pay");

                entity.Property(e => e.OtherNames)
                    .HasMaxLength(45)
                    .HasColumnName("Other Names");

                entity.Property(e => e.Others).HasColumnType("money");

                entity.Property(e => e.Period).HasMaxLength(20);

                entity.Property(e => e.SupplierNumber).HasColumnName("Supplier Number");

                entity.Property(e => e.TMShares)
                    .HasColumnType("money")
                    .HasColumnName("T M Shares");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.TotalKgs).HasColumnName("TOTAL(Kgs)");

                entity.Property(e => e.Trader).HasMaxLength(3);

                entity.Property(e => e.Transport).HasColumnType("money");
            });

            modelBuilder.Entity<QueryPayrollQuery>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("QueryPayroll Query");

                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(35)
                    .HasColumnName("Account Number");

                entity.Property(e => e.Advance).HasColumnType("money");

                entity.Property(e => e.Agrovet).HasColumnType("money");

                entity.Property(e => e.Ai)
                    .HasColumnType("money")
                    .HasColumnName("AI");

                entity.Property(e => e.BankName)
                    .HasMaxLength(45)
                    .HasColumnName("Bank Name");

                entity.Property(e => e.Branch).HasMaxLength(35);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("First Name");

                entity.Property(e => e.GrossPay)
                    .HasColumnType("money")
                    .HasColumnName("Gross Pay");

                entity.Property(e => e.HShares)
                    .HasColumnType("money")
                    .HasColumnName("H Shares");

                entity.Property(e => e.IdNumber)
                    .HasMaxLength(25)
                    .HasColumnName("Id Number");

                entity.Property(e => e.NetPay)
                    .HasColumnType("money")
                    .HasColumnName("Net Pay");

                entity.Property(e => e.OtherNames)
                    .HasMaxLength(45)
                    .HasColumnName("Other Names");

                entity.Property(e => e.Others).HasColumnType("money");

                entity.Property(e => e.Period).HasMaxLength(20);

                entity.Property(e => e.SupplierNumber).HasColumnName("Supplier Number");

                entity.Property(e => e.TMShares)
                    .HasColumnType("money")
                    .HasColumnName("T M Shares");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.TotalKgs).HasColumnName("TOTAL(Kgs)");

                entity.Property(e => e.Trader).HasMaxLength(3);

                entity.Property(e => e.Transport).HasColumnType("money");
            });

            modelBuilder.Entity<Rcpno>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("rcpno");

                entity.Property(e => e.Rcpno1).HasColumnName("Rcpno");
            });

            modelBuilder.Entity<ReceiptBooking>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ReceiptBooking");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Ccode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Chequeno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Companycode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("companycode")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Craccno)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Datedeposited)
                    .HasColumnType("datetime")
                    .HasColumnName("datedeposited");

                entity.Property(e => e.Draccno)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Memberno)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Posted)
                    .IsRequired()
                    .HasColumnName("posted")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Ptype)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ReceiptNo)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Ref)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.RefNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Transactionno)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Transdate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Receiptno>(entity =>
            {
                entity.HasKey(e => e.Receipthnoid);

                entity.ToTable("Receiptno");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Receiptno1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Receiptno");
            });

            modelBuilder.Entity<Reportpath>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("reportpath");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Reportpath1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("reportpath");
            });

            modelBuilder.Entity<Serialno>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("serialno");

                entity.Property(e => e.AuditDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("audit_date");

                entity.Property(e => e.PCode)
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.Serialid).HasColumnName("serialid");

                entity.Property(e => e.Serialno1)
                    .HasMaxLength(50)
                    .HasColumnName("serialno");

                entity.Property(e => e.Used).HasColumnName("used");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Session");

                entity.Property(e => e.Activity)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dDate");

                entity.Property(e => e.Dtime)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("dtime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.SessionId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("session_ID");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Description).HasMaxLength(35);

                entity.Property(e => e.Edate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("EDate");

                entity.Property(e => e.Period).HasMaxLength(25);
            });

            modelBuilder.Entity<Share>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Location).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Period).HasMaxLength(255);

                entity.Property(e => e.Refunds).HasColumnType("money");

                entity.Property(e => e.Sno).HasColumnName("SNo");
            });

            modelBuilder.Entity<SharesUpdate>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Period).HasMaxLength(255);

                entity.Property(e => e.Timestamp).HasColumnType("smalldatetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(25);
            });

            modelBuilder.Entity<Sisold>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("sisold");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PCode)
                    .HasMaxLength(50)
                    .HasColumnName("p_code");

                entity.Property(e => e.RNo)
                    .HasMaxLength(50)
                    .HasColumnName("r_no");

                entity.Property(e => e.SNo)
                    .HasMaxLength(50)
                    .HasColumnName("S_no");

                entity.Property(e => e.Supplier)
                    .HasMaxLength(50)
                    .HasColumnName("supplier");

                entity.Property(e => e.Transdate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("transdate");
            });

            modelBuilder.Entity<Smssubscription>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SMSSubscription");

                entity.Property(e => e.AuditDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Freq)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LastSent)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('22/03/2011')");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Subscription)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sno>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("sno");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Receipthnoid).ValueGeneratedOnAdd();

                entity.Property(e => e.Receiptno)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Staff1>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Staffname)
                    .HasMaxLength(50)
                    .HasColumnName("staffname");

                entity.Property(e => e.Staffno)
                    .HasMaxLength(50)
                    .HasColumnName("staffno");
            });

            modelBuilder.Entity<StaffPayroll>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("StaffPayroll");

                entity.Property(e => e.AccountNumber).HasMaxLength(255);

                entity.Property(e => e.Advances).HasColumnType("money");

                entity.Property(e => e.BankName).HasMaxLength(255);

                entity.Property(e => e.Basic).HasColumnType("money");

                entity.Property(e => e.Branch).HasMaxLength(255);

                entity.Property(e => e.EmpNumber).HasMaxLength(255);

                entity.Property(e => e.GrossPay).HasColumnType("money");

                entity.Property(e => e.HouseAllowance)
                    .HasColumnType("money")
                    .HasColumnName("House Allowance");

                entity.Property(e => e.MAllowance)
                    .HasColumnType("money")
                    .HasColumnName("M Allowance");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.Property(e => e.NetSalary)
                    .HasColumnType("money")
                    .HasColumnName("Net Salary");

                entity.Property(e => e.Nhif)
                    .HasColumnType("money")
                    .HasColumnName("NHIF");

                entity.Property(e => e.Nhifnumber)
                    .HasMaxLength(255)
                    .HasColumnName("NHIFNumber");

                entity.Property(e => e.Nssf)
                    .HasColumnType("money")
                    .HasColumnName("NSSF");

                entity.Property(e => e.NssfEmployer)
                    .HasColumnType("money")
                    .HasColumnName("NSSF Employer");

                entity.Property(e => e.Nssfnumber)
                    .HasMaxLength(255)
                    .HasColumnName("NSSFNumber");

                entity.Property(e => e.OtherBenefits)
                    .HasColumnType("money")
                    .HasColumnName("Other Benefits");

                entity.Property(e => e.OtherDeductions)
                    .HasColumnType("money")
                    .HasColumnName("Other Deductions");

                entity.Property(e => e.Overtime).HasColumnType("money");

                entity.Property(e => e.PayPoint)
                    .HasMaxLength(25)
                    .HasColumnName("Pay Point");

                entity.Property(e => e.Paye)
                    .HasColumnType("money")
                    .HasColumnName("PAYE");

                entity.Property(e => e.Period).HasMaxLength(20);

                entity.Property(e => e.Pin)
                    .HasMaxLength(25)
                    .HasColumnName("PIN");

                entity.Property(e => e.Sdr)
                    .HasColumnType("money")
                    .HasColumnName("SDR");

                entity.Property(e => e.SpfEmployer)
                    .HasColumnType("money")
                    .HasColumnName("SPF Employer");

                entity.Property(e => e.TotalDeductions)
                    .HasColumnType("money")
                    .HasColumnName("Total Deductions");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Status");

                entity.Property(e => e.State).HasMaxLength(50);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierNumber);

                entity.HasIndex(e => e.IdNumber, "Id Number");

                entity.Property(e => e.SupplierNumber)
                    .ValueGeneratedNever()
                    .HasColumnName("Supplier Number");

                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(35)
                    .HasColumnName("Account Number");

                entity.Property(e => e.BankName)
                    .HasMaxLength(45)
                    .HasColumnName("Bank Name");

                entity.Property(e => e.Branch).HasMaxLength(35);

                entity.Property(e => e.BranchSupplier).HasMaxLength(50);

                entity.Property(e => e.DateBegan)
                    .HasColumnType("datetime")
                    .HasColumnName("Date Began");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("First Name");

                entity.Property(e => e.IdNumber)
                    .HasMaxLength(25)
                    .HasColumnName("Id Number");

                entity.Property(e => e.Location).HasMaxLength(255);

                entity.Property(e => e.Names)
                    .HasMaxLength(97)
                    .HasComputedColumnSql("([FIRST NAME] + ' ' + isnull(([OTHER NAMES] + ' '),''))", false);

                entity.Property(e => e.OtherNames)
                    .HasMaxLength(45)
                    .HasColumnName("Other Names");

                entity.Property(e => e.RegisteredBy)
                    .HasMaxLength(35)
                    .HasColumnName("Registered By");

                entity.Property(e => e.Trader).HasMaxLength(3);

                entity.Property(e => e.Types).HasMaxLength(255);
            });

            modelBuilder.Entity<SwiftMessage>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Swift_Messages");

                entity.Property(e => e.Auditdate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Message).HasColumnType("text");

                entity.Property(e => e.Msgstatus).HasColumnName("msgstatus");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sysparam>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SYSPARAM");

                entity.Property(e => e.AuditId)
                    .HasMaxLength(10)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime).HasColumnType("smalldatetime");

                entity.Property(e => e.BankInterest).HasColumnType("money");

                entity.Property(e => e.ByLaws).HasColumnType("money");

                entity.Property(e => e.CompanyName).HasMaxLength(50);

                entity.Property(e => e.DeductAmt).HasColumnType("money");

                entity.Property(e => e.DefFundId)
                    .HasMaxLength(20)
                    .HasColumnName("DefFundID");

                entity.Property(e => e.DormancyPeriod).HasColumnType("money");

                entity.Property(e => e.GuarShareRatio).HasColumnType("money");

                entity.Property(e => e.InsPremium).HasColumnType("money");

                entity.Property(e => e.LoanInterest).HasColumnType("money");

                entity.Property(e => e.LoanToShareRatio).HasColumnType("money");

                entity.Property(e => e.Loandefaulted).HasColumnType("money");

                entity.Property(e => e.MinTotShares).HasColumnType("money");

                entity.Property(e => e.RegFees).HasColumnType("money");

                entity.Property(e => e.SelfGuar).HasMaxLength(5);

                entity.Property(e => e.ShareCapital).HasColumnType("money");

                entity.Property(e => e.ShareInterest).HasColumnType("money");

                entity.Property(e => e.Withdrawalnotice).HasColumnName("withdrawalnotice");
            });

            modelBuilder.Entity<Tbbalance>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tbbalance");

                entity.Property(e => e.AccGroup).HasMaxLength(50);

                entity.Property(e => e.AccType).HasMaxLength(50);

                entity.Property(e => e.Accname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("accname");

                entity.Property(e => e.Accno)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("accno");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("amount");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.BudgetAmount).HasColumnType("money");

                entity.Property(e => e.Closed).HasColumnName("closed");

                entity.Property(e => e.Cr)
                    .HasColumnType("money")
                    .HasColumnName("CR");

                entity.Property(e => e.Dr)
                    .HasColumnType("money")
                    .HasColumnName("DR");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Obal)
                    .HasColumnType("money")
                    .HasColumnName("OBal");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Transdate).HasColumnType("datetime");

                entity.Property(e => e.Transtype)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("transtype")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<TblMenu>(entity =>
            {
                entity.HasKey(e => e.Alias);

                entity.ToTable("tbl_menus");

                entity.Property(e => e.Alias)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Enabled).HasDefaultValueSql("(0)");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Menu)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RegDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblTfscostcentre>(entity =>
            {
                entity.HasKey(e => e.Cstctrid);

                entity.ToTable("TBL_TFSCOSTCENTRES");

                entity.HasIndex(e => e.Cstctrname, "IX_TBL_TFSCOSTCENTRES")
                    .IsUnique();

                entity.Property(e => e.Cstctrid)
                    .ValueGeneratedNever()
                    .HasColumnName("CSTCTRID");

                entity.Property(e => e.Cstctrdescription)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CSTCTRDESCRIPTION");

                entity.Property(e => e.Cstctrname)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("CSTCTRNAME");

                entity.Property(e => e.DexchOrgcode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("DEXCH_ORGCODE");

                entity.Property(e => e.DexchTtype)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("DEXCH_TTYPE")
                    .IsFixedLength(true);

                entity.Property(e => e.FactcodeFk)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("FACTCODE_FK");

                entity.Property(e => e.Synchro)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("__SYNCHRO")
                    .HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<TblUsermenu>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tbl_usermenus");

                entity.Property(e => e.Enable).HasColumnName("enable");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Menu)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("menu");

                entity.Property(e => e.Regdate)
                    .HasColumnType("datetime")
                    .HasColumnName("regdate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Userloginid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userloginid");
            });

            modelBuilder.Entity<TchpRate>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Tchp_Rate");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Rate).HasColumnType("money");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<TempGlTransaction>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tempGlTransactions");

                entity.Property(e => e.AccNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Chequeno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DocumentNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransDescript)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Transdate).HasColumnType("datetime");

                entity.Property(e => e.Transtype)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Tempcashbook>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TEMPCASHBOOK");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.DMonth)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("dMonth");

                entity.Property(e => e.DYear)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("dYear");

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("mCode");

                entity.Property(e => e.MName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("mName");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransType)
                    .IsRequired()
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<Tempmemberstatement>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TEMPMEMBERSTATEMENT");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Interest).HasColumnType("money");

                entity.Property(e => e.MemberNo).HasMaxLength(20);

                entity.Property(e => e.MonthlyContr).HasColumnType("money");

                entity.Property(e => e.Principal).HasColumnType("money");

                entity.Property(e => e.RefNo).HasMaxLength(50);

                entity.Property(e => e.Total).HasColumnType("money");
            });

            modelBuilder.Entity<Temttbbalance>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TEMTTBBALANCE");

                entity.Property(e => e.AccNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Budget)
                    .HasColumnType("money")
                    .HasColumnName("budget");

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransType)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TEST");

                entity.Property(e => e.Pcode)
                    .HasMaxLength(10)
                    .HasColumnName("PCODE")
                    .IsFixedLength(true);

                entity.Property(e => e.Qin)
                    .HasMaxLength(10)
                    .HasColumnName("QIN")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Tmpaginganalysis>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TMPAGINGANALYSIS");

                entity.Property(e => e.Balance)
                    .HasColumnType("money")
                    .HasColumnName("BALANCE");

                entity.Property(e => e.Companycode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("COMPANYCODE");

                entity.Property(e => e.Companyname)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("COMPANYNAME");

                entity.Property(e => e.Lastdate)
                    .HasColumnType("datetime")
                    .HasColumnName("LASTDATE");

                entity.Property(e => e.LoanNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MemberNo)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.OtherNames)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Repayrate)
                    .HasColumnType("money")
                    .HasColumnName("REPAYRATE");

                entity.Property(e => e.Surname)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tmpcashbook>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TMPCASHBOOK");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransType)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TotalShare>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("First Name");

                entity.Property(e => e.IdNumber)
                    .HasMaxLength(25)
                    .HasColumnName("Id Number");

                entity.Property(e => e.Location).HasMaxLength(255);

                entity.Property(e => e.OtherNames)
                    .HasMaxLength(45)
                    .HasColumnName("Other Names");

                entity.Property(e => e.Refunds).HasColumnType("money");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.TotalAmount).HasColumnType("money");
            });

            modelBuilder.Entity<TrackChange>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.SupplierNumber).HasColumnName("Supplier Number");

                entity.Property(e => e.Timestamp).HasColumnType("smalldatetime");

                entity.Property(e => e.User).HasMaxLength(35);
            });

            modelBuilder.Entity<Tran>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TRANS$");

                entity.Property(e => e.Active).HasMaxLength(255);

                entity.Property(e => e.Auditdatetime)
                    .HasMaxLength(255)
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(255)
                    .HasColumnName("auditid");

                entity.Property(e => e.DateInactivate).HasMaxLength(255);

                entity.Property(e => e.Isfrate)
                    .HasMaxLength(255)
                    .HasColumnName("isfrate");

                entity.Property(e => e.Rate).HasMaxLength(255);

                entity.Property(e => e.Sno).HasMaxLength(255);

                entity.Property(e => e.Startdate)
                    .HasMaxLength(255)
                    .HasColumnName("startdate");

                entity.Property(e => e.TransCode)
                    .HasMaxLength(255)
                    .HasColumnName("Trans_Code");
            });

            modelBuilder.Entity<TransCode>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TransCode");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Transactioncode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AuditId)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.AuditTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasDefaultValueSql("(N'Active')");

                entity.Property(e => e.TransDate).HasColumnType("datetime");

                entity.Property(e => e.TransDescription)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("transDescription");

                entity.Property(e => e.TransactionNo)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Transport>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Transport");

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Code).HasMaxLength(3);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Frate).HasColumnName("frate");

                entity.Property(e => e.Isfrate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("isfrate");

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(255)
                    .HasColumnName("Supplier Name");

                entity.Property(e => e.SupplierNumber).HasColumnName("Supplier Number");
            });

            modelBuilder.Entity<Transporter>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.HasIndex(e => e.IdNumber, "Id Number");

                entity.Property(e => e.Code).HasMaxLength(3);

                entity.Property(e => e.AccNumber)
                    .HasMaxLength(25)
                    .HasColumnName("Acc Number");

                entity.Property(e => e.BankName)
                    .HasMaxLength(255)
                    .HasColumnName("Bank Name");

                entity.Property(e => e.Branch).HasMaxLength(255);

                entity.Property(e => e.BranchTransporter).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Frate).HasColumnName("frate");

                entity.Property(e => e.IdNumber).HasColumnName("Id Number");

                entity.Property(e => e.Isfrate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("isfrate");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Rate).HasColumnName("rate");
            });

            modelBuilder.Entity<Transporter1>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("Transporters$");

                entity.Property(e => e.Code).HasMaxLength(255);

                entity.Property(e => e.AccNumber)
                    .HasMaxLength(255)
                    .HasColumnName("Acc Number");

                entity.Property(e => e.BankName)
                    .HasMaxLength(255)
                    .HasColumnName("Bank Name");

                entity.Property(e => e.Branch).HasMaxLength(255);

                entity.Property(e => e.BranchTransporter).HasMaxLength(255);

                entity.Property(e => e.Date).HasColumnType("smalldatetime");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IdNumber).HasColumnName("Id Number");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<TransportersPayQuery>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TransportersPayQuery");

                entity.Property(e => e.AccNumber)
                    .HasMaxLength(25)
                    .HasColumnName("Acc Number");

                entity.Property(e => e.Advances).HasColumnType("money");

                entity.Property(e => e.Agrovet).HasColumnType("money");

                entity.Property(e => e.Ai)
                    .HasColumnType("money")
                    .HasColumnName("AI");

                entity.Property(e => e.Amnt).HasColumnType("money");

                entity.Property(e => e.BankName)
                    .HasMaxLength(255)
                    .HasColumnName("Bank Name");

                entity.Property(e => e.Branch).HasMaxLength(255);

                entity.Property(e => e.Code).HasMaxLength(3);

                entity.Property(e => e.GrossPay).HasColumnType("money");

                entity.Property(e => e.HShares)
                    .HasColumnType("money")
                    .HasColumnName("H Shares");

                entity.Property(e => e.IdNumber).HasColumnName("Id Number");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.NetPay).HasColumnType("money");

                entity.Property(e => e.Others).HasColumnType("money");

                entity.Property(e => e.Period).HasMaxLength(25);

                entity.Property(e => e.Subsidy).HasColumnType("money");

                entity.Property(e => e.TMShares)
                    .HasColumnType("money")
                    .HasColumnName("T M Shares");

                entity.Property(e => e.TotalDeductions)
                    .HasColumnType("money")
                    .HasColumnName("Total deductions");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("USERS");

                entity.Property(e => e.GroupId)
                    .HasMaxLength(10)
                    .HasColumnName("GroupID");

                entity.Property(e => e.IsSuperUser).HasMaxLength(3);

                entity.Property(e => e.UserId)
                    .HasMaxLength(10)
                    .HasColumnName("UserID");

                entity.Property(e => e.UserPassword).HasMaxLength(10);
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasKey(e => e.Userid);

                entity.Property(e => e.AssignGl)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Branch)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Branchcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("branchcode");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.Levels)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("levels");

                entity.Property(e => e.PassExpire)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Sign)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("sign");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Superuser).HasColumnName("SUPERUSER");

                entity.Property(e => e.UserGroup)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UserLoginIds)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UserLoginIDs");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Userid)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("USERid");
            });

            modelBuilder.Entity<UserAccounts1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserAccounts1");

                entity.Property(e => e.AssignGl)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.Branchcode)
                    .HasMaxLength(50)
                    .HasColumnName("branchcode");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DepCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Department)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Euser)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("euser");

                entity.Property(e => e.Expirydate)
                    .HasMaxLength(50)
                    .HasColumnName("expirydate");

                entity.Property(e => e.Levels)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("levels");

                entity.Property(e => e.PassExpire)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.PasswordStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("passwordStatus");

                entity.Property(e => e.Sign)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("sign");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Superuser).HasColumnName("SUPERUSER");

                entity.Property(e => e.UserGroup).HasMaxLength(50);

                entity.Property(e => e.UserLoginId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("UserLoginID");

                entity.Property(e => e.UserName).HasMaxLength(250);

                entity.Property(e => e.Userstatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userstatus");

                entity.Property(e => e.VendorId).HasColumnName("vendor_ID");
            });

            modelBuilder.Entity<Usergroup>(entity =>
            {
                entity.HasKey(e => e.GroupId);

                entity.ToTable("USERGROUPS");

                entity.Property(e => e.Files).HasColumnName("files");

                entity.Property(e => e.GroupId)
                    .HasMaxLength(30)
                    .HasColumnName("GroupID");

                entity.Property(e => e.GroupName).HasMaxLength(50);
            });

            modelBuilder.Entity<Usergrp>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("USERGRPS");

                entity.Property(e => e.Activate)
                    .HasMaxLength(50)
                    .HasColumnName("activate");

                entity.Property(e => e.Archived).HasMaxLength(5);

                entity.Property(e => e.AuditId)
                    .HasMaxLength(10)
                    .HasColumnName("AuditID");

                entity.Property(e => e.AuditTime).HasColumnType("datetime");

                entity.Property(e => e.Backup)
                    .HasMaxLength(50)
                    .HasColumnName("backup");

                entity.Property(e => e.Banksetup)
                    .HasMaxLength(5)
                    .HasColumnName("banksetup");

                entity.Property(e => e.BenevolentFund)
                    .HasMaxLength(50)
                    .HasColumnName("benevolentFund");

                entity.Property(e => e.Calc)
                    .HasMaxLength(5)
                    .HasColumnName("calc");

                entity.Property(e => e.Changememno)
                    .HasMaxLength(5)
                    .HasColumnName("changememno");

                entity.Property(e => e.Chequeentry)
                    .HasMaxLength(5)
                    .HasColumnName("chequeentry");

                entity.Property(e => e.Clearloanrecs)
                    .HasMaxLength(5)
                    .HasColumnName("clearloanrecs");

                entity.Property(e => e.Clearmemrecs)
                    .HasMaxLength(5)
                    .HasColumnName("clearmemrecs");

                entity.Property(e => e.Companysetup)
                    .HasMaxLength(5)
                    .HasColumnName("companysetup");

                entity.Property(e => e.Contributions)
                    .HasMaxLength(50)
                    .HasColumnName("contributions");

                entity.Property(e => e.Databasesetup)
                    .HasMaxLength(5)
                    .HasColumnName("databasesetup");

                entity.Property(e => e.Deductions)
                    .HasMaxLength(5)
                    .HasColumnName("deductions");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Dividends)
                    .HasMaxLength(5)
                    .HasColumnName("dividends");

                entity.Property(e => e.Dormant).HasMaxLength(5);

                entity.Property(e => e.EffectRepayment)
                    .HasMaxLength(5)
                    .HasColumnName("effectRepayment");

                entity.Property(e => e.ExportToGl)
                    .HasMaxLength(5)
                    .HasColumnName("exportToGL");

                entity.Property(e => e.GroupId)
                    .HasMaxLength(10)
                    .HasColumnName("GroupID");

                entity.Property(e => e.LoanBal)
                    .HasMaxLength(5)
                    .HasColumnName("loanBal");

                entity.Property(e => e.LoanGuarantors)
                    .HasMaxLength(5)
                    .HasColumnName("loanGuarantors");

                entity.Property(e => e.LoanTrans)
                    .HasMaxLength(5)
                    .HasColumnName("loanTrans");

                entity.Property(e => e.Loanapplic)
                    .HasMaxLength(5)
                    .HasColumnName("loanapplic");

                entity.Property(e => e.Loanendorsement)
                    .HasMaxLength(5)
                    .HasColumnName("loanendorsement");

                entity.Property(e => e.Loantypes)
                    .HasMaxLength(5)
                    .HasColumnName("loantypes");

                entity.Property(e => e.MemReg).HasMaxLength(5);

                entity.Property(e => e.MemStatement).HasMaxLength(5);

                entity.Property(e => e.Monthlydeductions)
                    .HasMaxLength(5)
                    .HasColumnName("monthlydeductions");

                entity.Property(e => e.Nok)
                    .HasMaxLength(5)
                    .HasColumnName("NOK");

                entity.Property(e => e.Parametization)
                    .HasMaxLength(5)
                    .HasColumnName("parametization");

                entity.Property(e => e.Periodictran)
                    .HasMaxLength(5)
                    .HasColumnName("periodictran");

                entity.Property(e => e.Rejreasons)
                    .HasMaxLength(5)
                    .HasColumnName("rejreasons");

                entity.Property(e => e.Rwbanking)
                    .HasMaxLength(5)
                    .HasColumnName("RWBanking");

                entity.Property(e => e.Rwloans)
                    .HasMaxLength(5)
                    .HasColumnName("RWLoans");

                entity.Property(e => e.Rwmembers)
                    .HasMaxLength(50)
                    .HasColumnName("RWMembers");

                entity.Property(e => e.RwotherSchemes)
                    .HasMaxLength(50)
                    .HasColumnName("RWOtherSchemes");

                entity.Property(e => e.Rwsetup)
                    .HasMaxLength(5)
                    .HasColumnName("RWSetup");

                entity.Property(e => e.Rwshares)
                    .HasMaxLength(5)
                    .HasColumnName("RWShares");

                entity.Property(e => e.Rwutilities)
                    .HasMaxLength(5)
                    .HasColumnName("RWUtilities");

                entity.Property(e => e.Savings)
                    .HasMaxLength(50)
                    .HasColumnName("savings");

                entity.Property(e => e.Sharevar)
                    .HasMaxLength(50)
                    .HasColumnName("sharevar");

                entity.Property(e => e.Sysusers)
                    .HasMaxLength(5)
                    .HasColumnName("sysusers");

                entity.Property(e => e.Usergrps)
                    .HasMaxLength(5)
                    .HasColumnName("usergrps");

                entity.Property(e => e.UtilGuarantor)
                    .HasMaxLength(5)
                    .HasColumnName("utilGuarantor");

                entity.Property(e => e.Utilstatements)
                    .HasMaxLength(5)
                    .HasColumnName("utilstatements");

                entity.Property(e => e.Withdrawn).HasMaxLength(5);
            });

            modelBuilder.Entity<View1>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VIEW1");
            });

            modelBuilder.Entity<Vwpartly>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vwpartly");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Branch)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateDeduc)
                    .HasColumnType("datetime")
                    .HasColumnName("Date_Deduc");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Period)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Remarks)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Yyear).HasColumnName("yyear");
            });

            modelBuilder.Entity<Vwpartlytran>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Vwpartlytrans");

                entity.Property(e => e.Ai).HasColumnName("AI");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Auditdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("auditdatetime");

                entity.Property(e => e.Auditid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("auditid");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Enddate)
                    .HasColumnType("datetime")
                    .HasColumnName("enddate");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Period).HasMaxLength(25);

                entity.Property(e => e.Rate).HasColumnName("rate");

                entity.Property(e => e.Remarks).HasMaxLength(50);

                entity.Property(e => e.Startdate)
                    .HasColumnType("datetime")
                    .HasColumnName("startdate");

                entity.Property(e => e.TdateDeduc)
                    .HasColumnType("datetime")
                    .HasColumnName("TDate_Deduc");

                entity.Property(e => e.TransCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Yyear).HasColumnName("yyear");
            });

            modelBuilder.Entity<Vwshare>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vwshares");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("amount");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Names)
                    .HasMaxLength(85)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("SNo");
            });

            modelBuilder.Entity<Vwsharebal>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vwsharebal");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("amount");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Names)
                    .HasMaxLength(85)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("SNo");
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Staff");

                entity.Property(e => e.AccountNumber).HasMaxLength(25);

                entity.Property(e => e.BankName).HasMaxLength(40);

                entity.Property(e => e.Branch).HasMaxLength(40);

                entity.Property(e => e.DateEmployeed)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Date Employeed");

                entity.Property(e => e.EmployeeNumber).HasMaxLength(255);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.Property(e => e.Nhifnumber)
                    .HasMaxLength(255)
                    .HasColumnName("NHIFNumber");

                entity.Property(e => e.Nssfnumber)
                    .HasMaxLength(255)
                    .HasColumnName("NSSFNumber");

                entity.Property(e => e.Phone).HasMaxLength(40);

                entity.Property(e => e.Pin)
                    .HasMaxLength(25)
                    .HasColumnName("PIN");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
