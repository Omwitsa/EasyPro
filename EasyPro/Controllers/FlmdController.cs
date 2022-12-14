using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class FlmdController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public FlmdController(MORINGAContext context, INotyfService notyf)
        {
            utilities = new Utilities(context);
            _context = context;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.FLMD.Where(s => s.SaccoCode== sacco).ToListAsync());
        }

        public IActionResult Create(string sno)
        {
            utilities.SetUpPrivileges(this);
            ViewBag.sno = sno ?? "0";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var suppliers = _context.DSuppliers
               .Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.suppliers = suppliers;
            return View();
        }

        public IActionResult Details(string id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var suppliers = _context.DSuppliers
               .Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.suppliers = suppliers;
            return RedirectToAction("Create", new { sno = id });
        }

        [HttpPost]
        public JsonResult UpdateAnimals([FromBody] FLMD fMLD)
        {
            try
            {
                if (string.IsNullOrEmpty(fMLD.Sno))
                    return Json("");
                utilities.SetUpPrivileges(this);
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                fMLD.SaccoCode = sacco;
                var savedFlmd = _context.FLMD.FirstOrDefault(f => f.Sno.ToUpper().Equals(fMLD.Sno.ToUpper()) && f.SaccoCode == sacco);
                if (savedFlmd == null)
                    _context.FLMD.Add(fMLD);
                else
                {
                    savedFlmd.ExoticCattle = fMLD.ExoticCattle;
                    savedFlmd.IndigenousCattle = fMLD.IndigenousCattle;
                    savedFlmd.IndigenousChicken = fMLD.IndigenousChicken;
                    savedFlmd.Sheep = fMLD.Sheep;
                    savedFlmd.Goats = fMLD.Goats;
                    savedFlmd.Camels = fMLD.Camels;
                    savedFlmd.Donkeys = fMLD.Donkeys;
                    savedFlmd.Pigs = fMLD.Pigs;
                    savedFlmd.BeeHives = fMLD.BeeHives;
                }
                _context.SaveChanges();
                _notyf.Success("Animals saved successfully");
                return Json("");
            }
            catch (Exception e)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

        [HttpGet]
        public JsonResult FlmdDetails(string sno)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";

                var flmd = _context.FLMD.FirstOrDefault(f => f.Sno == sno.ToUpper() && f.SaccoCode == sacco);
                var crops = _context.FLMDCrops.Where(c => c.Sno == sno.ToUpper() && c.SaccoCode == sacco).ToList();
                var lands = _context.FLMDLand.Where(d => d.Sno == sno.ToUpper() && d.SaccoCode == sacco).ToList();

                return Json(new
                {
                    flmd,
                    crops,
                    lands
                });
            }
            catch (Exception e)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult UpdateEducation([FromBody] FLMD fMLD)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                if (string.IsNullOrEmpty(fMLD.Sno))
                    return Json("");
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                fMLD.SaccoCode = sacco;
                var savedFlmd = _context.FLMD.FirstOrDefault(f => f.Sno.ToUpper().Equals(fMLD.Sno.ToUpper()) && f.SaccoCode == sacco);
                if (savedFlmd == null)
                    _context.FLMD.Add(fMLD);
                else
                {
                    savedFlmd.Boys = fMLD.Boys;
                    savedFlmd.Girls = fMLD.Girls;
                    savedFlmd.Deaths = fMLD.Deaths;
                    savedFlmd.Disabled = fMLD.Disabled;
                    savedFlmd.PostGraduates = fMLD.PostGraduates;
                    savedFlmd.Graduates = fMLD.Graduates;
                    savedFlmd.UnderGraduates = fMLD.UnderGraduates;
                    savedFlmd.Secondary = fMLD.Secondary;
                    savedFlmd.Primary = fMLD.Primary;
                    savedFlmd.Nursery = fMLD.Nursery;
                }
                _context.SaveChanges();
                _notyf.Success("Education Levels saved successfully");
                return Json("");
            }
            catch (Exception e)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult UpdateCrops([FromBody] FLMDCrops crops)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                if (string.IsNullOrEmpty(crops.Sno))
                    return Json("");
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                crops.SaccoCode = sacco;
                _context.FLMDCrops.Add(crops);
                _context.SaveChanges();
                _notyf.Success("Crops saved successfully");

                var fLMDCrops = _context.FLMDCrops.Where(c => c.Sno == crops.Sno.ToUpper() && c.SaccoCode == sacco).ToList();
                return Json(fLMDCrops);
            }
            catch (Exception e)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult updateLand([FromBody] FLMDLand land)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                if (string.IsNullOrEmpty(land.Sno))
                    return Json("");
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                land.SaccoCode = sacco;
                _context.FLMDLand.Add(land);
                _context.SaveChanges();
                _notyf.Success("Land saved successfully");

                var fLMDLands = _context.FLMDLand.Where(c => c.Sno == land.Sno.ToUpper() && c.SaccoCode == sacco).ToList();
                return Json(fLMDLands);
            }
            catch (Exception e)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }
    }
}
