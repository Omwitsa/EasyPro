using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using System;
using ClosedXML.Excel;
using System.IO;
using NPOI.SS.Formula.Functions;

namespace EasyPro.Controllers
{
    public class DTransportersController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DTransportersController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        public DTransporter dtransporterobj { get; set; }
        // GET: DTransporters
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            GetInitialValues();
            var transporters = _context.DTransporters
                .Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper()));

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(t => t.Tbranch == saccoBranch);
            return View(await transporters.ToListAsync());
        }

        // GET: DTransporters/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dTransporter = await _context.DTransporters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransporter == null)
            {
                return NotFound();
            }

            return View(dTransporter);
        }

        // GET: DTransporters/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var banksname = _context.DBanks.Where(i=>i.BankCode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.BankName).ToList();
            ViewBag.banksname = new SelectList(banksname);

            var brances = _context.DBranch.Where(i => i.Bcode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToList();
            ViewBag.brances = new SelectList(brances);

            var bankbrances = _context.DBankBranch.Where(i => i.BankCode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToList();
            ViewBag.bankbrances = new SelectList(bankbrances);

            var locations = _context.DLocations.Where(i => i.Lcode.ToUpper().Equals(sacco.ToUpper()) && i.Branch== saccoBranch).Select(b => b.Lname).ToList();
            ViewBag.locations = new SelectList(locations);

            var zones = _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToList();
            ViewBag.zones = new SelectList(zones);

            var Routes = _context.Routes.Where(a => a.scode == sacco).Select(b => b.Name).ToList();
            ViewBag.Route = new SelectList(Routes);
            //var zone = _context.Zones.Where(a => a.Code == sacco).ToList();
            if (Routes.Count != 0)
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;

            List<SelectListItem> gender = new()
            {
                new SelectListItem { Text = "" },
                new SelectListItem { Text = "Male" },
                new SelectListItem {Text = "Female" },
            };
            ViewBag.gender = gender;
            List<SelectListItem> payment = new()
            {
                new SelectListItem { Text = "" },
                new SelectListItem {  Text = "Weekly" },
                new SelectListItem { Text = "Monthly" },
            };
            ViewBag.payment = payment;
        }
        // POST: DTransporters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransCode,TransName,CertNo,Locations,TregDate,Email,Phoneno,Town,Address,Subsidy,Accno,Bcode,Bbranch,Active,Tbranch,Auditid,Auditdatetime,Isfrate,Rate,Canno,Tt,ParentT,Ttrate,Br,Freezed,PaymenMode")] DTransporter dTransporter)
        
        
        
        
        
        
        
        
        
        
        
        
        
        {
            try
            {
                utilities.SetUpPrivileges(this);
                if (dTransporter == null)
                {
                    _notyf.Error("Transporter cannot be empty");
                    return NotFound();
                }
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
                sacco = sacco ?? "";
                var dTransporterExists = _context.DTransporters
                    .Any(i => (i.TransCode == dTransporter.TransCode || i.CertNo == dTransporter.CertNo)
                    && i.ParentT.ToUpper().Equals(sacco.ToUpper()) && i.Tbranch == saccoBranch);
                if (dTransporterExists)
                {
                    GetInitialValues();
                    _notyf.Error("Transporter entered already exist");
                    return View();
                }
                if (ModelState.IsValid)
                {
                    dTransporter.ParentT = sacco;
                    dTransporter.Tbranch = saccoBranch;
                    _context.Add(dTransporter);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Transporter saved successfully");
                    return RedirectToAction(nameof(Index));
                }
                return View(dTransporter);
            }
            catch (Exception ex)
            {
                _notyf.Error("Sorry, An error occurred");
                return View(dTransporter);
            }
        }

        // GET: DTransporters/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }
            GetInitialValues();
            var dTransporter = await _context.DTransporters.FindAsync(id);
            if (dTransporter == null)
            {
                return NotFound();
            }
            return View(dTransporter);
        }
        
        public async Task<IActionResult> ExportFamers(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                _notyf.Error("Sorry, Transpoter not found");
                return NotFound();
            }
            var dTransporter = await _context.DTransporters.FindAsync(id);
            if (dTransporter == null)
            {
                _notyf.Error("Sorry, Transpoter not found");
                return NotFound();
            }
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var company = _context.DCompanies.FirstOrDefault(u => u.Name == sacco);
            using (var workbook = new XLWorkbook())
            {
                
                var worksheet = workbook.Worksheets.Add("Farmers");
                var currentRow = 1;
                if(company != null)
                {
                    worksheet.Cell(currentRow, 2).Value = company.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = company.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = company.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = company.Email;
                }
               
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Transporters farmers";
                worksheet.Cell(currentRow, 3).Value = dTransporter.TransCode;
                worksheet.Cell(currentRow, 4).Value = dTransporter.TransName;

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "Phone No";
                worksheet.Cell(currentRow, 4).Value = "IDNo";
                worksheet.Cell(currentRow, 5).Value = "Branch";
                decimal sum = 0;

                var supplierNos = _context.DTransports.Where(s => s.TransCode == dTransporter.TransCode
                && s.saccocode == sacco).Select(s => s.Sno).ToList();
                var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco && s.Branch == saccoBranch
                && supplierNos.Contains(s.Sno)).ToList();
                foreach (var supplier in suppliers)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = supplier.Sno;
                    worksheet.Cell(currentRow, 2).Value = supplier.Names;
                    worksheet.Cell(currentRow, 3).Value = supplier.PhoneNo;
                    worksheet.Cell(currentRow, 4).Value = supplier.IdNo;
                    worksheet.Cell(currentRow, 5).Value = supplier.Branch;
                    sum = sum + 1;
                }
                currentRow++;
                worksheet.Cell(currentRow, 4).Value = "No of Farmers";
                worksheet.Cell(currentRow, 5).Value = sum;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Transporters Farmers.xlsx");
                }
            }
        }

        // POST: DTransporters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TransCode,TransName,CertNo,Locations,TregDate,Email,Phoneno,Town,Address,Subsidy,Accno,Bcode,Bbranch,Active,Tbranch,Auditid,Auditdatetime,Isfrate,Rate,Canno,Tt,ParentT,Ttrate,Br,Freezed,PaymenMode")] DTransporter dTransporter)
        {
            utilities.SetUpPrivileges(this);
            if (id != dTransporter.Id)
            {
                _notyf.Error("Sorry, error occured while editing, try again");
                return NotFound();
            }
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";

            if (ModelState.IsValid)
            {
                try
                {
                    dTransporter.TransCode = dTransporter.TransCode;
                    dTransporter.Br = "A";
                    dTransporter.Freezed = "0";
                    dTransporter.ParentT = sacco;
                    _context.Update(dTransporter);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Transporter Edited successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTransporterExists(dTransporter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dTransporter);
        }

        // GET: DTransporters/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dTransporter = await _context.DTransporters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransporter == null)
            {
                return NotFound();
            }

            return View(dTransporter);
        }

        // POST: DTransporters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dTransporter = await _context.DTransporters.FindAsync(id);
            _context.DTransporters.Remove(dTransporter);
            await _context.SaveChangesAsync();
            _notyf.Success("Transporter Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool DTransporterExists(long id)
        {
            return _context.DTransporters.Any(e => e.Id == id);
        }
    }
}
