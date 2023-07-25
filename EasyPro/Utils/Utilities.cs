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
                    Flmd = true
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
    }
}
