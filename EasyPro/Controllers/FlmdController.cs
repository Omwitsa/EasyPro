using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

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
        }

        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public JsonResult UpdateAnimals([FromBody] FLMD fMLD, string sno)
        {
            try
            {
                sno = sno ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                fMLD.SaccoCode = sacco;
                var savedFlmd = _context.FLMD.FirstOrDefault(f => f.Sno.ToUpper().Equals(sno.ToUpper()));
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

        [HttpPost]
        public JsonResult UpdateEducation([FromBody] FLMD fMLD, string sno)
        {
            try
            {
                sno = sno ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                fMLD.SaccoCode = sacco;
                var savedFlmd = _context.FLMD.FirstOrDefault(f => f.Sno.ToUpper().Equals(sno.ToUpper()));
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
        public JsonResult UpdateCrops([FromBody] FLMDCrops crops, string sno)
        {
            try
            {
                sno = sno ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                crops.SaccoCode = sacco;
                _context.FLMDCrops.Add(crops);
                _context.SaveChanges();
                _notyf.Success("Crops saved successfully");

                var fLMDCrops = _context.FLMDCrops.Where(c => c.Sno == sno).ToList();
                return Json(fLMDCrops);
            }
            catch (Exception e)
            {
                _notyf.Error("Sorry, An error occurred");
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult updateLand([FromBody] FLMDLand land, string sno)
        {
            try
            {
                sno = sno ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                land.SaccoCode = sacco;
                _context.FLMDLand.Add(land);
                _context.SaveChanges();
                _notyf.Success("Land saved successfully");

                var fLMDLands = _context.FLMDLand.Where(c => c.Sno == sno).ToList();
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
