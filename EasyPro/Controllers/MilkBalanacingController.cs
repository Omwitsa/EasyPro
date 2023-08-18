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
            GetInitialValues();
            utilities.SetUpPrivileges(this);
            PostTransporterIntake();

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var suppliers = await _context.TransportersBalancings
                .Where(i => i.Code.ToUpper().Equals(sacco.ToUpper()) && i.Date >= startDate
                && i.Date <= endDate).ToListAsync();
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(u => u.Branch == saccoBranch).ToList();


            suppliers = suppliers.OrderByDescending(s => s.Date).ToList();
            return View(suppliers);
        }

        public void PostTransporterIntake()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var transporterIntakes = _context.d_TransporterIntake.Where(i => i.Posted != "Posted"
            && i.SaccoCode == sacco && i.Branch== saccobranch).ToList();
            var intakes = transporterIntakes.GroupBy(i => i.Date).ToList();
            intakes.ForEach(i =>
            {
                var transGroups = i.GroupBy(t => t.TransCode).ToList();
                transGroups.ForEach(t =>
                {
                    var intake = t.FirstOrDefault();
                    var qty = t.Sum(t => t.ActualKg);
                    var savedIntakes = GetTransporterIntakes(intake.TransCode, intake.Date);
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
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            GetInitialValues();
            

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
            var agproducts = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            var TransportersName = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if(user.AccessLevel == AccessLevel.Branch)
            {
                agproducts = agproducts.Where(i => i.Tbranch == saccobranch).ToList();
                TransportersName = TransportersName.Where(t => t.Tbranch == saccobranch).ToList();
            }
            ViewBag.agproductsall = agproducts;
            ViewBag.Transporterslist = new SelectList(TransportersName, "TransName", "TransName");
            if(sacco.ToUpper()== "MBURUGU DAIRY F.C.S")
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;
        }

        [HttpPost]
        public JsonResult GetSuppliedItems([FromBody] ProductBalancingFilterVm filter)
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

                var suppliersdeliveries = _context.d_TransporterIntake.Where(i => i.Date == filter.Date && i.TransCode.ToUpper().Equals(filter.TCode.ToUpper())
                && i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch).Sum(i => i.ActualKg);
                var intakes = GetTransporterIntakes(filter.TCode, filter.Date);
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

        private IEnumerable<ProductIntake> GetTransporterIntakes(string transCode, DateTime? date)
        {
            transCode = transCode ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var transports = _context.DTransports.Where(t => t.TransCode.ToUpper().Equals(transCode.ToUpper()) 
            && t.saccocode == sacco);
            var intakes = _context.ProductIntake.Where(i => i.SaccoCode == sacco);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                transports = transports.Where(s => s.Branch == saccobranch);
                intakes = intakes.Where(i => i.Branch == saccobranch);
            }
            var transporterSuppliers = transports.Select(t => t.Sno);

            var notTransporterSuppliers = intakes.Where(i => i.AuditId.ToUpper().Equals(transCode.ToUpper()) 
                && !transporterSuppliers.Contains(i.Sno.ToUpper()) && i.TransDate == date)
                .Select(t => t.Sno).Distinct().ToList();

            intakes = intakes.Where(s => s.TransDate == date && (s.Description == "Intake" || s.Description == "Correction")
            && (transporterSuppliers.Contains(s.Sno) || notTransporterSuppliers.Contains(s.Sno))).OrderByDescending(h => h.Auditdatetime);

            return intakes.ToList();
        }


        [HttpPost]//editVariance
        public JsonResult SaveVariance([FromBody] TransportersBalancing balancing ,bool print)
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
                var checkexist = _context.TransportersBalancings
                    .FirstOrDefault(s => s.Date == balancing.Date && s.Code == sacco && s.Branch== saccoBranch && s.Transporter == balancing.Transporter);
                if (checkexist!=null)
                {
                    var id = checkexist.Id;
                    var transportersBalancing = _context.TransportersBalancings.Find(id);
                    _context.TransportersBalancings.Remove(transportersBalancing);
                }

                var checkIfexist = _context.ProductIntake
                    .Where(s => s.TransDate >= startDate && s.TransDate <= enDate && s.SaccoCode == sacco && s.Branch == saccoBranch
                    && s.Sno == balancing.Transporter && s.ProductType == "variance").ToList();
                checkIfexist.ForEach(g =>
                {
                    var id = g.Id;
                    var intaketransportersBalancing = _context.ProductIntake.Find(id);
                    _context.ProductIntake.RemoveRange(intaketransportersBalancing);
                });

                balancing.Code = sacco;
                    balancing.Branch = saccoBranch;
                    _context.TransportersBalancings.Add(balancing);
               
                _context.SaveChanges();

                //calc variance kgs
                decimal systemkgs = 0;
                decimal kgsdelivered = 0;
                decimal Vvariance = 0;
                var sumdeliveredkgs = _context.TransportersBalancings
                    .Where(s => s.Date >= startDate && s.Date <= enDate && s.Code == sacco
                    && s.Branch == saccoBranch && s.Transporter == balancing.Transporter).ToList();
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
