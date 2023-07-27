using EasyPro.Constants;
using EasyPro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace EasyPro.Utils
{
    public class Utilities
    {
        private MORINGAContext _context;
        public Utilities(MORINGAContext context)
        {
            _context = context;
        }
        public void SetUpPrivileges(Controller controller)
        {
            var sacco = controller.HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var group = controller.HttpContext.Session.GetString(StrValues.UserGroup) ?? "";
            var usergroup = _context.Usergroups.FirstOrDefault(u => u.GroupName.Equals(group)
            && u.SaccoCode.ToUpper().Equals(sacco.ToUpper()));

            //create a default user group of admin for new society
            if (usergroup == null)
            {
                var val = new Usergroup
                {
                    GroupId = group,
                    GroupName = group,
                    Registration = true,
                    Activity = true,
                    Reports = true,
                    Setup = true,
                    Files = true,
                    Accounts = true,
                    Deductions = true,
                    SaccoReports = false,
                    SaccoCode = sacco,
                    Staff = true,
                    Store = true,
                    Flmd = true,
                    RFarmers = true,
                    RTransporter = true,
                    RImportS = true,
                    RVendor = true,
                    RCustomer = true,
                    StandingOrder = true,
                    CashShares = true,
                    DefaultDed = true,
                    DedFarmer = true,
                    DedTransport = true,
                    DedStaff = true,
                    TransporterAssign = true,
                    Millers = true,
                    Marketers = true,
                    Pulping = true,
                    Milling = true,
                    Marketing = true,
                    CofPricing = true,
                    CofPayroll = true,
                    SetProducts = true,
                    SetPrice = true,
                    SetFarmersDif = true,
                    SetOrganization = true,
                    SetOrgBranch = true,
                    SetUsers = true,
                    SetUserGroups = true,
                    SetCounties = true,
                    SetSubCounties = true,
                    SetWards = true,
                    SetLocation = true,
                    SetDedTypes = true,
                    SetBanks = true,
                    SetBanksBranch = true,
                    SetZones = true,
                    SetDebtors = true,
                    SetTaxes = true,
                    SetSharesCat = true,
                    SetRoutes = true,
                    Products = true,
                    ProdSupplier = true,
                    ProdSales = true,
                    SalesReturn=true,
                    ProdDispatch=true,
                    ProdIntake = true,
                    IntakeCorrection = true,
                    ImportIntake = true,
                    MilkTest = true,
                    VarBalancing = true,
                    Dispatch = true,
                    SendSms = true,
                    MilkControl = true,
                    BranchMilkEnquiry = true,
                    SupplierStatement = true,
                    TransporterStatement = true,
                    ChartsofAcc = true,
                    JournalPosting = true,
                    Glinquiry = true,
                    Budgettings = true,
                    JournalListing = true,
                    TrialBalance = true,
                    IncomeStatement = true,
                    BalanceSheet = true,
                    Payroll = true,
                    Bills = true,
                    Refunds = true,
                    CustomerInvoices = true,
                    CreditNotes = true,
                    VendorProducts = true,
                    CustomerProducts = true
                };

                _context.Usergroups.Add(val);
                _context.SaveChanges();

                usergroup = _context.Usergroups.FirstOrDefault(u => u.GroupName.Equals(group)
                       && u.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
            }
            //end 
            
           
            controller.ViewBag.sacco = sacco;
            controller.ViewBag.filesRole = usergroup.Files;
            controller.ViewBag.accountsRole = usergroup.Accounts;
            controller.ViewBag.transactionsRole = usergroup.Registration;
            controller.ViewBag.activityRole = usergroup.Activity;
            controller.ViewBag.setupRole = usergroup.Setup;
            controller.ViewBag.reportsRole = usergroup.Reports;
            controller.ViewBag.saccoReportsRole = usergroup.SaccoReports;
            controller.ViewBag.staffRole = usergroup.Staff;
            controller.ViewBag.stockRole = usergroup.Store;
            controller.ViewBag.deductionsRole = usergroup.Deductions;
            controller.ViewBag.flmd = usergroup.Flmd;
            controller.ViewBag.isTanyakina = sacco == StrValues.Tanykina;
            controller.ViewBag.RFarmers = usergroup.RFarmers;
            controller.ViewBag.RTransporter = usergroup.RTransporter;
            controller.ViewBag.RImportS = usergroup.RImportS;
            controller.ViewBag.RVendor = usergroup.RVendor;
            controller.ViewBag.RCustomer = usergroup.RCustomer;
            controller.ViewBag.StandingOrder = usergroup.StandingOrder;
            controller.ViewBag.CashShares = usergroup.CashShares;
            controller.ViewBag.DefaultDed = usergroup.DefaultDed;
            controller.ViewBag.DedFarmer = usergroup.DedFarmer;
            controller.ViewBag.DedTransport = usergroup.DedTransport;
            controller.ViewBag.DedStaff = usergroup.DedStaff;
            controller.ViewBag.TransporterAssign = usergroup.TransporterAssign;
            controller.ViewBag.Millers = usergroup.Millers;
            controller.ViewBag.Marketers = usergroup.Marketers;
            controller.ViewBag.Pulping = usergroup.Pulping;
            controller.ViewBag.Milling = usergroup.Milling;
            controller.ViewBag.Marketing = usergroup.Marketing;
            controller.ViewBag.CofPricing = usergroup.CofPricing;
            controller.ViewBag.CofPayroll = usergroup.CofPayroll;
            controller.ViewBag.SetProducts = usergroup.SetProducts;
            controller.ViewBag.SetPrice = usergroup.SetPrice;
            controller.ViewBag.SetFarmersDif = usergroup.SetFarmersDif;
            controller.ViewBag.SetOrganization = usergroup.SetOrganization;
            controller.ViewBag.SetOrgBranch = usergroup.SetOrgBranch;
            controller.ViewBag.SetUsers = usergroup.SetUsers;
            controller.ViewBag.SetUserGroups = usergroup.SetUserGroups;
            controller.ViewBag.SetCounties = usergroup.SetCounties;
            controller.ViewBag.SetSubCounties = usergroup.SetSubCounties;
            controller.ViewBag.SetWards = usergroup.SetWards;
            controller.ViewBag.SetLocation = usergroup.SetLocation;
            controller.ViewBag.SetDedTypes = usergroup.SetDedTypes;
            controller.ViewBag.SetBanks = usergroup.SetBanks;
            controller.ViewBag.SetBanksBranch = usergroup.SetBanksBranch;
            controller.ViewBag.SetZones = usergroup.SetZones;
            controller.ViewBag.SetDebtors = usergroup.SetDebtors;
            controller.ViewBag.SetTaxes = usergroup.SetTaxes;
            controller.ViewBag.SetSharesCat = usergroup.SetSharesCat;
            controller.ViewBag.SetRoutes = usergroup.SetRoutes;
            controller.ViewBag.productsRole = usergroup.Products;
            controller.ViewBag.productSupplierRole = usergroup.ProdSupplier;
            controller.ViewBag.productSalesRole = usergroup.ProdSales;
            controller.ViewBag.salesReturnRole = usergroup.SalesReturn;
            controller.ViewBag.prodDispatchRole = usergroup.ProdDispatch;
            controller.ViewBag.ProdIntakeRole = usergroup.ProdIntake;
            controller.ViewBag.CorrectionRole = usergroup.IntakeCorrection;
            controller.ViewBag.ImportIntakeRole = usergroup.ImportIntake;
            controller.ViewBag.MilkTestRole = usergroup.MilkTest;
            controller.ViewBag.VarBalancingRole = usergroup.VarBalancing;
            controller.ViewBag.DispatchRole = usergroup.Dispatch;
            controller.ViewBag.SendSmsRole = usergroup.SendSms;
            controller.ViewBag.MilkControlRole = usergroup.MilkControl;
            controller.ViewBag.BranchMilkEnquiryRole = usergroup.BranchMilkEnquiry;
            controller.ViewBag.SupplierStatementRole = usergroup.SupplierStatement;
            controller.ViewBag.TransporterStatementRole = usergroup.TransporterStatement;
            controller.ViewBag.ChartsofAccRole = usergroup.ChartsofAcc;
            controller.ViewBag.JournalPostingRole = usergroup.JournalPosting;
            controller.ViewBag.GlinquiryRole = usergroup.Glinquiry;
            controller.ViewBag.BudgettingsRole = usergroup.Budgettings;
            controller.ViewBag.JournalListingRole = usergroup.JournalListing;
            controller.ViewBag.TrialBalanceRole = usergroup.TrialBalance;
            controller.ViewBag.IncomeStatementRole = usergroup.IncomeStatement;
            controller.ViewBag.BalanceSheetRole = usergroup.BalanceSheet;
            controller.ViewBag.PayrollRole = usergroup.Payroll;
            controller.ViewBag.BillsRole = usergroup.Bills;
            controller.ViewBag.RefundsRole = usergroup.Refunds;
            controller.ViewBag.CustomerInvoicesRole = usergroup.CustomerInvoices;
            controller.ViewBag.CreditNotesRole = usergroup.CreditNotes;
            controller.ViewBag.VendorProductsRole = usergroup.VendorProducts;
            controller.ViewBag.CustomerProductsRole = usergroup.CustomerProducts;
        }

        public string GenerateExcelGridSupReg(ISheet sheet, string sacco, string loggedInUser, string branch)
        {
            StringBuilder sb = new StringBuilder();
            IRow headerRow = sheet.GetRow(0); //Get Header Row
            int cellCount = headerRow.LastCellNum;
            sb.Append("<table class='table table-bordered'><tr>");
            for (int j = 0; j < cellCount; j++)
            {
                NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                sb.Append("<th>" + cell.ToString() + "</th>");
            }
            sb.AppendLine("</tr>");
            sb.Append("<tr>");
            var existingData = _context.ExcelDumpSupReg.Where(d => d.LoggedInUser == loggedInUser && d.SaccoCode == sacco).ToList();
            if(existingData.Any())
                _context.ExcelDumpSupReg.RemoveRange(existingData);

            decimal totalQnty = 0;
            var excelDumps = new List<ExcelDumpSupReg>();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                }
                // Reg_date, SNo, Names, PhoneNo, IdNo, DOB, Acc_Number, Bank_code, Bank_Branch,
                // Gender, PaymentMode, Village, LOCATION, WARD, SUB_COUNTY, COUNTY, LoggedInUser, Branch, SaccoCode
                decimal.TryParse(row.GetCell(2).ToString(), out decimal qnty);
                totalQnty += qnty;
                String dateString = (row.GetCell(0)).ToString();
                DateTime transDate = DateTime.Parse(dateString);

                var DobdateString = row.GetCell(5).ToString();
                DateTime DobtransDate = DateTime.Parse(DobdateString);

                excelDumps.Add(new ExcelDumpSupReg
                {
                    LoggedInUser = loggedInUser,
                    SaccoCode = sacco,
                    Branch = branch,
                    Reg_date = transDate,
                    SNo = row.GetCell(1)?.ToString() ?? "",
                    Names = row.GetCell(2)?.ToString() ?? "",
                    PhoneNo = row.GetCell(3)?.ToString() ?? "",
                    IdNo = row.GetCell(4)?.ToString() ?? "",
                    DOB = DobtransDate,
                    Acc_Number = row.GetCell(6)?.ToString() ?? "",
                    Bank_code = row.GetCell(7)?.ToString() ?? "",
                    Bank_Branch = row.GetCell(8)?.ToString() ?? "",
                    Gender = row.GetCell(9)?.ToString() ?? "",
                    PaymentMode = row.GetCell(10)?.ToString() ?? "",
                    Village = row.GetCell(11)?.ToString() ?? "",
                    LOCATION = row.GetCell(12)?.ToString() ?? "",
                    WARD = row.GetCell(13)?.ToString() ?? "",
                    SUB_COUNTY = row.GetCell(14)?.ToString() ?? "",
                    COUNTY = row.GetCell(15)?.ToString() ?? "",
                });
                
                sb.AppendLine("</tr>");
            }

            if (excelDumps.Any())
            {
                _context.ExcelDumpSupReg.AddRange(excelDumps);
                _context.SaveChanges();
            }


            sb.Append("<tr>");
            for (int j = 0; j < cellCount; j++)
            {
                if(j == 0)
                    sb.Append("<td>Total</td>");
                else if(j == 2)
                    sb.Append("<td>" + totalQnty + "</td>");
                else
                    sb.Append("<td></td>");
            }
            
            sb.AppendLine("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }
        public string GenerateExcelGrid(ISheet sheet, string sacco, string loggedInUser, string branch)
        {
            StringBuilder sb = new StringBuilder();
            IRow headerRow = sheet.GetRow(0); //Get Header Row
            int cellCount = headerRow.LastCellNum;
            sb.Append("<table class='table table-bordered'><tr>");
            for (int j = 0; j < cellCount; j++)
            {
                NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                sb.Append("<th>" + cell.ToString() + "</th>");
            }
            sb.AppendLine("</tr>");
            sb.Append("<tr>");
            var existingData = _context.ExcelDump.Where(d => d.LoggedInUser == loggedInUser && d.SaccoCode == sacco).ToList();
            if (existingData.Any())
                _context.ExcelDump.RemoveRange(existingData);

            decimal totalQnty = 0;
            var excelDumps = new List<ExcelDump>();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                }

                decimal.TryParse(row.GetCell(2).ToString(), out decimal qnty);
                totalQnty += qnty;
                var dateString = row.GetCell(3).ToString();
                var transDate = DateTime.Parse(dateString);
                excelDumps.Add(new ExcelDump
                {
                    LoggedInUser = loggedInUser,
                    SaccoCode = sacco,
                    Branch = branch,
                    Sno = row.GetCell(0)?.ToString() ?? "",
                    ProductType = row.GetCell(1)?.ToString() ?? "",
                    Quantity = qnty,
                    TransDate = transDate,
                    TransCode = row.GetCell(4)?.ToString() ?? ""
                });

                sb.AppendLine("</tr>");
            }

            if (excelDumps.Any())
            {
                _context.ExcelDump.AddRange(excelDumps);
                _context.SaveChanges();
            }


            sb.Append("<tr>");
            for (int j = 0; j < cellCount; j++)
            {
                if (j == 0)
                    sb.Append("<td>Total</td>");
                else if (j == 2)
                    sb.Append("<td>" + totalQnty + "</td>");
                else
                    sb.Append("<td></td>");
            }

            sb.AppendLine("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }
        public decimal? GetBalance(ProductIntake productIntake)
        {
            var latestIntake = _context.ProductIntake.Where(i => i.Sno == productIntake.Sno && i.SaccoCode.ToUpper().Equals(productIntake.SaccoCode.ToUpper()))
                    .OrderByDescending(i => i.Id).FirstOrDefault();
            if (latestIntake == null)
                latestIntake = new ProductIntake();
            latestIntake.Balance = latestIntake?.Balance ?? 0;
            productIntake.DR = productIntake?.DR ?? 0;
            productIntake.CR = productIntake?.CR ?? 0;
            var balance = latestIntake.Balance + productIntake.CR - productIntake.DR;
            return balance;
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public string GenerateDeductionsExcelGrid(ISheet sheet, string sacco, string loggedInUser, string branch)
        {
            StringBuilder sb = new StringBuilder();
            IRow headerRow = sheet.GetRow(0); //Get Header Row
            int cellCount = headerRow.LastCellNum;
            sb.Append("<table class='table table-bordered'><tr>");
            for (int j = 0; j < cellCount; j++)
            {
                NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                sb.Append("<th>" + cell.ToString() + "</th>");
            }
            sb.AppendLine("</tr>");
            sb.Append("<tr>");

            var existingData = _context.ExcelDeductionDump.Where(d => d.LoggedInUser == loggedInUser && d.SaccoCode == sacco).ToList();
            if (existingData.Any())
                _context.ExcelDeductionDump.RemoveRange(existingData);

            decimal totalAmount = 0;
            var excelDumps = new List<ExcelDeductionDump>();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                }

                decimal.TryParse(row.GetCell(2).ToString(), out decimal amount);
                totalAmount += amount;
                var dateString = row.GetCell(3).ToString();
                var transDate = DateTime.Parse(dateString);
                excelDumps.Add(new ExcelDeductionDump
                {
                    Sno = row.GetCell(0)?.ToString() ?? "",
                    ProductType = row.GetCell(1)?.ToString() ?? "",
                    Amount = amount,
                    TransDate = transDate,
                    LoggedInUser = loggedInUser,
                    SaccoCode = sacco,
                    Branch = branch
                });

                sb.AppendLine("</tr>");
            }

            if (excelDumps.Any())
            {
                _context.ExcelDeductionDump.AddRange(excelDumps);
                _context.SaveChanges();
            }


            sb.Append("<tr>");
            for (int j = 0; j < cellCount; j++)
            {
                if (j == 0)
                    sb.Append("<td>Total</td>");
                else if (j == 2)
                    sb.Append("<td>" + totalAmount + "</td>");
                else
                    sb.Append("<td></td>");
            }

            sb.AppendLine("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }

    }
}
