using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Linq;
using System.Text;

namespace EasyPro.Controllers
{
    public class ExcelUploadController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private IWebHostEnvironment _hostingEnvironment;
        private Utilities utilities;

        public ExcelUploadController(MORINGAContext context, INotyfService notyf, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _notyf = notyf;
            _hostingEnvironment = hostingEnvironment;
            utilities = new Utilities(context);
        }

        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";

            return View();
        }

        public ActionResult Import()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";

            IFormFile file = Request.Form.Files[0];
            string folderName = "UploadExcel";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                    }
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
                    double totalAge = 0;
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

                        var va = row.GetCell(2).ToString();
                        double.TryParse(row.GetCell(2).ToString(), out double age);
                        totalAge += age;
                        sb.AppendLine("</tr>");
                    }
                    sb.Append("<tr>");
                    sb.Append("<td>Total</td>");
                    sb.Append("<td></td>");
                    sb.Append("<td>" + totalAge + "</td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.AppendLine("</tr>");
                    sb.Append("</table>");
                }
            }
            return this.Content(sb.ToString());
        }

        public ActionResult Download()
        {
            string Files = "wwwroot/UploadExcel/CoreProgramm_ExcelImport.xlsx";
            byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
            System.IO.File.WriteAllBytes(Files, fileBytes);
            MemoryStream ms = new MemoryStream(fileBytes);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "employee.xlsx");
        }
    }
}
