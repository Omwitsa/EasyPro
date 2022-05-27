using EasyPro.Models;
using EasyPro.Repository;
using EasyPro.Utils;
using EasyPro.ViewModels.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class ReportController : Controller
    {
        readonly IReporting _IReporting;
        private Utilities utilities;
        public ReportController(IReporting iReporting, MORINGAContext context)
        {
            _IReporting = iReporting;
            utilities = new Utilities(context);
        }
        [HttpGet]
        public IActionResult DownloadReport()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }
        [HttpGet]
        public IActionResult DownloadReport1()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DownloadReport(IFormCollection obj)
        {
            utilities.SetUpPrivileges(this);
            //string reportname = $"User_Wise_{Guid.NewGuid():N}.xlsx";
            string reportname = $"Farmers List.xlsx";
            var list = _IReporting.GetUserwiseReport();
            if (list.Count > 0)
            {
                var exportbytes = ExporttoExcel<UserMasterViewModel>(list, reportname);
                return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
            }
            else
            {
                TempData["Message"] = "No Data to Export";
                return View();
            }
        } //
        [HttpPost]
        public IActionResult DownloadReport2(IFormCollection obj)
        {
            utilities.SetUpPrivileges(this);
            //string reportname = $"User_Wise_{Guid.NewGuid():N}.xlsx"; UserDeducViewModel
            string reportname = $"Suppliers Deduc.xlsx";
            var list = _IReporting.GetUserdeductionReport();
            if (list.Count > 0)
            {
                var exportbytes = ExporttoExcel<UserDeduViewModel>(list, reportname);
                return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
            }
            else
            {
                TempData["Message"] = "No Data to Export";
                return View();
            }
        }

        [HttpPost]
        public IActionResult DownloadReport1(IFormCollection obj)
        {
            utilities.SetUpPrivileges(this);
            //string reportname = $"User_Wise_{Guid.NewGuid():N}.xlsx";
            string reportname = $"Suppliers Intake.xlsx";
            var list = _IReporting.GetUserintakeReport();
            if (list.Count > 0)
            {
                var exportbytes = ExporttoExcel<UserIntakeViewModel>(list, reportname);
                return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
            }
            else
            {
                TempData["Message"] = "No Data to Export";
                return View();
            }
        }
        private byte[] ExporttoExcel<T>(List<T> table, string filename)
    {
        using ExcelPackage pack = new ExcelPackage();
        ExcelWorksheet ws = pack.Workbook.Worksheets.Add(filename);
        ws.Cells["A1"].LoadFromCollection(table, true, TableStyles.Light1);
        return pack.GetAsByteArray();
    }
}
}
