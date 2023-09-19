using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class MilkBalancingController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private IWebHostEnvironment _hostingEnvironment;
        private Utilities utilities;

        public MilkBalancingController(MORINGAContext context, INotyfService notyf, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _notyf = notyf;
            _hostingEnvironment = hostingEnvironment;
            utilities = new Utilities(context);
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            GetInitialValues();
            utilities.SetUpPrivileges(this);
            await PostTransporterIntake();

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var suppliers = await _context.TransportersBalancings
                .Where(i => i.Code.ToUpper().Equals(sacco.ToUpper()) && i.Date >= startDate
                && i.Date <= endDate).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(u => u.Branch == saccoBranch).ToList();


            suppliers = suppliers.OrderByDescending(s => s.Date).ToList();
            return View(suppliers);
        }

        public async Task PostTransporterIntake()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var transporterIntakes = await _context.d_TransporterIntake.Where(i => i.Posted != "Posted"
            && i.SaccoCode == sacco).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                transporterIntakes = transporterIntakes.Where(s => s.Branch == saccobranch).ToList();

            var intakes = transporterIntakes.GroupBy(i => i.Date).ToList();
            intakes.ForEach(i =>
            {
                var transGroups = i.GroupBy(t => t.TransCode).ToList();
                transGroups.ForEach(async t =>
                {
                    var intake = t.FirstOrDefault();
                    var qty = t.Sum(t => t.ActualKg);
                    var savedIntakes = await GetTransporterIntakes(intake.TransCode, intake.Date);
                    var supplies = savedIntakes.Sum(s => s.Qsupplied);
                    var variance = supplies - qty;
                    _context.TransportersBalancings.Add(new TransportersBalancing
                    {
                        Date = (DateTime)intake.Date,
                        Transporter = intake.TransCode,
                        Quantity = "" + supplies,
                        ActualBal = "" + qty,
                        Rejects = "0",
                        Spillage = "0",
                        Varriance = "" + variance,
                        Code = sacco,
                        Branch = saccobranch,
                    });

                });

                i.ToList().ForEach(t => t.Posted = "Posted");
                _context.SaveChanges();
            });
        }

        public IActionResult Create(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            ViewBag.remarks = StrValues.Slopes == sacco ? "Invoice No." : "Remarks";
            return View(new TransportersBalancing
            {
                Spillage = "0",
                Varriance = "0",
                Rejects = "0",
            });
        }
        // GET: DSuppliers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id == null)
            {
                return NotFound();
            }
            var TransportersBalancings = await _context.TransportersBalancings.FindAsync(id);
            if (TransportersBalancings == null)
            {
                return NotFound();
            }
            var transporter = _context.DTransporters.FirstOrDefault(i=>i.TransCode.ToUpper().Equals(TransportersBalancings.Transporter.ToUpper())
            && i.ParentT== TransportersBalancings.Code && i.Tbranch== TransportersBalancings.Branch);


            ViewBag.TransportersBalancings = TransportersBalancings;
            //return View(ViewBag.TransportersBalancings);

            return View(TransportersBalancings);
        }
        // GET: DSuppliers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var TransportersBalancings = await _context.TransportersBalancings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (TransportersBalancings == null)
            {
                return NotFound();
            }

            return View(TransportersBalancings);
        }

        // POST: DSuppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var dSupplier = await _context.TransportersBalancings.FindAsync(id);
            _context.TransportersBalancings.Remove(dSupplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var transporters = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if(user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(t => t.Tbranch == saccobranch).ToList();
           
            ViewBag.agproductsall = transporters;
            var transporterNames = transporters.Select(t => t.TransName).ToList();
            if(StrValues.Slopes == sacco)
                transporterNames = transporters.Select(t => t.CertNo).ToList();
            ViewBag.Transporterslist = new SelectList(transporterNames);
            ViewBag.slopes = StrValues.Slopes == sacco;
            if (sacco == StrValues.Mburugu)
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;
        }

        [HttpPost]
        public async Task<JsonResult> GetSuppliedItems([FromBody] ProductBalancingFilterVm filter)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
                
                if (string.IsNullOrEmpty(filter.TCode))
                {
                    _notyf.Error("Sorry, Kindly select transporter");
                    return Json("");
                }

                var transporterIntakes = await _context.d_TransporterIntake.Where(i => i.Date == filter.Date 
                && i.TransCode.ToUpper().Equals(filter.TCode.ToUpper())
                && i.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
                var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
                if (user.AccessLevel == AccessLevel.Branch)
                    transporterIntakes = transporterIntakes.Where(s => s.Branch == saccobranch).ToList();

                var suppliersdeliveries = transporterIntakes.Sum(i => i.ActualKg);
                var intakes = await GetTransporterIntakes(filter.TCode, filter.Date);
                return Json(new {
                    intakes,
                    suppliersdeliveries
                });
            }
            catch (Exception e)
            {
                _notyf.Error("Sorry, an error occured");
                return Json("");
            }
        }

        private async Task<IEnumerable<ProductIntake>> GetTransporterIntakes(string transCode, DateTime? date)
        {
            transCode = transCode ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var transports = await _context.DTransports.Where(t => t.TransCode.ToUpper().Equals(transCode.ToUpper()) 
            && t.saccocode == sacco).ToListAsync();
            var intakes = await _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate == date).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                transports = transports.Where(s => s.Branch == saccobranch).ToList();
                intakes = intakes.Where(i => i.Branch == saccobranch).ToList();
            }

            var transporterSuppliers = transports.Select(t => t.Sno);

            var notTransporterSuppliers = intakes.Where(i => i.AuditId.ToUpper().Equals(transCode.ToUpper())
                && !transporterSuppliers.Contains(i.Sno.ToUpper()))
                .Select(t => t.Sno).Distinct().ToList();

            if (StrValues.Slopes == sacco) 
            {
                var auditDatetimes = intakes.Where(i => i.Sno.ToUpper().Equals(transCode.ToUpper()))
                    .Select(i => i.Auditdatetime);

                notTransporterSuppliers = intakes.Where(i => !transporterSuppliers.Contains(i.Sno.ToUpper()) 
                && auditDatetimes.Contains(i.Auditdatetime) && (i.Description == "Intake" || i.Description == "Correction"))
                .Select(t => t.Sno).Distinct().ToList();
            }

            intakes = intakes.Where(s => (s.Description == "Intake" || s.Description == "Correction")
            && (transporterSuppliers.Contains(s.Sno) || notTransporterSuppliers.Contains(s.Sno))).OrderBy(h => h.Auditdatetime).ToList();
            return intakes;
        }

        [HttpPost]//editVariance
        public async Task<JsonResult> SaveVariance([FromBody] TransportersBalancing balancing ,bool print)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";

                // Start printing your items.
                DateTime startDate = new DateTime(balancing.Date.Year, balancing.Date.Month, 1);
                DateTime enDate = startDate.AddMonths(1).AddDays(-1);

                if (string.IsNullOrEmpty(balancing.Transporter))
                {
                    _notyf.Error("Sorry, Kindly select transporter");
                    return Json("");
                }
                if (string.IsNullOrEmpty(balancing.Quantity))
                {
                    _notyf.Error("Sorry, Supplied items could not be found");
                    return Json("");
                }
                if (string.IsNullOrEmpty(balancing.ActualBal))
                {
                    _notyf.Error("Sorry, Kindy provide actuals");
                    return Json("");
                }

                var balancings = await _context.TransportersBalancings.Where(s => s.Date == balancing.Date && s.Code == sacco && s.Transporter == balancing.Transporter).ToListAsync();
                var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
                if (user.AccessLevel == AccessLevel.Branch)
                    balancings = balancings.Where(s => s.Branch == saccoBranch).ToList();

                var checkexist = balancings.FirstOrDefault();
                if (checkexist!=null)
                    _context.TransportersBalancings.Remove(checkexist);

                var checkIfexist = await _context.ProductIntake
                    .Where(s => s.TransDate >= startDate && s.TransDate <= enDate && s.SaccoCode == sacco
                    && s.Sno == balancing.Transporter && s.ProductType == "variance").ToListAsync();
                if (user.AccessLevel == AccessLevel.Branch)
                    checkIfexist = checkIfexist.Where(s => s.Branch == saccoBranch).ToList();

                if(checkIfexist.Any())
                    _context.ProductIntake.RemoveRange(checkIfexist);

                balancing.Code = sacco;
                balancing.Branch = saccoBranch;
                _context.TransportersBalancings.Add(balancing);
                _context.SaveChanges();

                var sumdeliveredkgs = await _context.TransportersBalancings
                    .Where(s => s.Date >= startDate && s.Date <= enDate && s.Code == sacco
                     && s.Transporter == balancing.Transporter).ToListAsync();
                if (user.AccessLevel == AccessLevel.Branch)
                    sumdeliveredkgs = sumdeliveredkgs.Where(s => s.Branch == saccoBranch).ToList();

                decimal systemkgs = 0;
                decimal kgsdelivered = 0;
                decimal Vvariance = 0;

                sumdeliveredkgs.ForEach(n =>
                {
                    systemkgs = systemkgs + Convert.ToDecimal(n.Quantity);
                    kgsdelivered = kgsdelivered + Convert.ToDecimal(n.ActualBal);
                });

                Vvariance = Convert.ToDecimal(kgsdelivered) - Convert.ToDecimal(systemkgs);
                decimal? price = _context.DPrices.FirstOrDefault(h => h.SaccoCode == sacco).Price;
                if (Vvariance < 0)
                {
                    var variance = Vvariance * price;
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = balancing.Transporter.ToUpper(),
                        TransDate = enDate,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = "variance",
                        Qsupplied = 0,
                        Ppu = 0,
                        CR = 0,
                        DR = variance * -1,
                        Balance = 0,
                        Description = "Variance Balancing",
                        TransactionType = TransactionType.Deduction,
                        Paid = false,
                        Remarks = "Variance Balancing",
                        AuditId = loggedInUser,
                        Auditdatetime = DateTime.Now,
                        Branch = saccoBranch,
                        SaccoCode = sacco,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = false
                    });
                }
                _context.SaveChanges();
                if (print)
                    PrintP(balancing);

                _notyf.Success("Saved successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        private IActionResult PrintP(TransportersBalancing balancing)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += 
                (sender, args) => printDocument_PrintPage(balancing, args);
            printDocument.Print();
            return Ok(200);
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var companies = _context.DCompanies.FirstOrDefault(i => i.Name.ToUpper().Equals(sacco.ToUpper()));

            TransportersBalancing items = sender as TransportersBalancing;
            if (items != null)
            {
                // Start printing your items.
                DateTime startDate = new DateTime(items.Date.Year, items.Date.Month, 1);
                DateTime enDate = startDate.AddMonths(1).AddDays(-1);

                //calc delivered kgs
                decimal systemkgs =0;
                decimal kgsdelivered = 0;
                decimal Vvariance = 0;
                var sumdeliveredkgs = _context.TransportersBalancings
                    .Where(s => s.Date >= startDate && s.Date <= enDate && s.Code == sacco
                    && s.Branch == saccoBranch && s.Transporter.Trim().ToUpper().Equals(items.Transporter.ToUpper())).ToList();
                sumdeliveredkgs.ForEach(n =>
                {
                    systemkgs = systemkgs + Convert.ToDecimal(n.Quantity);
                    kgsdelivered = kgsdelivered + Convert.ToDecimal(n.ActualBal);
                });

                Vvariance = Convert.ToDecimal(kgsdelivered)-Convert.ToDecimal(systemkgs) ;

                string transporter = transporter = _context.DTransporters.FirstOrDefault(u => u.ParentT.ToUpper().Equals(sacco.ToUpper()) &&
                 u.TransCode.Trim().ToUpper().Equals(items.Transporter.Trim().ToUpper()) && u.Active).TransName.ToString();

                Graphics graphics = e.Graphics;
                Font font = new Font("Times New Roman", 8);
                float fontHeight = font.GetHeight();

                int startX = 10;
                int startY = -40;
                int offset = 40;


                graphics.DrawString(companies.Name, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString(companies.Adress.PadLeft(10), font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString(companies.Town.PadLeft(10), font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString("Tell: " + companies.PhoneNo.PadLeft(10), font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString("Branch: " + saccoBranch, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string line = "---------------------------------------------";
                graphics.DrawString(line, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString("Transporters Balancing Receipt: ".PadRight(15), new Font("Times New Roman", 15), new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                var datet = items.Date.ToString("dd/MM/yyy");
                graphics.DrawString("Date : " + datet.PadRight(15), new Font("Times New Roman", 15), new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string sno = items.Code.PadRight(10);
                graphics.DrawString("TransCode: " + sno, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string name = transporter.ToString();
                graphics.DrawString("Name: " + name, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string kgs = string.Format("{0:.###}", items.ActualBal);
                graphics.DrawString("Actual Delivered: " + kgs + "kgs", font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string cummkgs = string.Format("{0:.###}", items.Quantity);
                graphics.DrawString("Farmers Kgs: " + cummkgs + "kgs", font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string Varriance = string.Format("{0:.###}",items.Varriance);
                graphics.DrawString("Varriance: " + Varriance + "kgs", font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string TVarriance = string.Format("{0:.###}", Vvariance);
                graphics.DrawString("Total Varriance: " + TVarriance + "kgs", font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string line1 = "---------------------------------------------";
                graphics.DrawString(line1, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString("Received By: " + loggedInUser, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString("Date: " + DateTime.Now, font, new SolidBrush(Color.Black), startX, startY + offset);

                offset = offset + (int)fontHeight + 5;

                string line2 = "---------------------------------------------";
                graphics.DrawString(line2, font, new SolidBrush(Color.Black), startX, startY + offset);

                offset = offset + (int)fontHeight + 5;
                startY = startY + 20;
                string dev = "DEVELOP BY: AMTECH TECHNOLOGIES LIMITED";
                graphics.DrawString(dev.PadRight(13), new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);

                offset = offset + (int)fontHeight + 5;
                startY = startY + 20;
                string dev1 = " ";
                graphics.DrawString(dev1.PadRight(13), new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;
                graphics.DrawString(line1, font, new SolidBrush(Color.Black), startX, startY + offset);

            }
        }


        [HttpPost]
        public JsonResult EditVariance([FromBody] TransportersBalancing balancing)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                if (string.IsNullOrEmpty(balancing.Transporter))
                {
                    _notyf.Error("Sorry, Kindly select transporter");
                    return Json("");
                }
                if (string.IsNullOrEmpty(balancing.Quantity))
                {
                    _notyf.Error("Sorry, Supplied items could not be found");
                    return Json("");
                }
                if (string.IsNullOrEmpty(balancing.ActualBal))
                {
                    _notyf.Error("Sorry, Kindy provide actuals");
                    return Json("");
                }

                balancing.Code = sacco;
                _context.TransportersBalancings.Update(balancing);
                _context.SaveChanges();
                _notyf.Success("Edited successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }
    }
}
