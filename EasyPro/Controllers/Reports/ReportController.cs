using EasyPro.IProvider;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportProvider _reportProvider;
        public ReportController(IReportProvider reportProvider)
        {
            _reportProvider = reportProvider;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var pdfFile = _reportProvider.GeneratePdfReport();
            return File(pdfFile,
            "application/octet-stream", "SimplePdf.pdf");
        }
    }
}
