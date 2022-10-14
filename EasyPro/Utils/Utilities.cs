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
            var group = controller.HttpContext.Session.GetString(StrValues.UserGroup) ?? "";
            var usergroup = _context.Usergroups.FirstOrDefault(u => u.GroupName.Equals(group));
            controller.ViewBag.sacco = controller.HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
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
        }

        public string GenerateExcelGrid(ISheet sheet, string sacco, string loggedInUser)
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
            if(existingData.Any())
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
                var transDate = DateTime.Parse(row.GetCell(3).ToString());
                excelDumps.Add(new ExcelDump
                {
                    LoggedInUser = loggedInUser,
                    SaccoCode = sacco,
                    Sno = row.GetCell(0).ToString(),
                    ProductType = row.GetCell(1).ToString(),
                    Quantity = qnty,
                    TransDate = transDate
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
