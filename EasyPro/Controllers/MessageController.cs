using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class MessageController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private IWebHostEnvironment _hostingEnvironment;
        private Utilities utilities;

        public MessageController(MORINGAContext context, INotyfService notyf, IWebHostEnvironment hostingEnvironment)
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

            var locations = _context.DLocations.Where(s => s.Lcode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.locations = new SelectList(locations, "Lname", "Lname");
            var branches = _context.DBranch.Where(t => t.Bcode == sacco).ToList();
            ViewBag.branches = new SelectList(branches, "Bname", "Bname");
            var transporters = _context.DTransporters.Where(t => t.ParentT == sacco).ToList();
            ViewBag.transporters = new SelectList(transporters, "TransCode", "TransName");
            var sharesCategories = _context.SharesCategories.Where(t => t.SaccoCode == sacco).ToList();
            ViewBag.sharesCategories = new SelectList(sharesCategories, "Name", "Name");
            return View();
        }

        [HttpPost]
        public JsonResult SmpPosting([FromBody] SmsVm sms)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                sms.Location = sms?.Location ?? "";
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                if (string.IsNullOrEmpty(sms.Content))
                {
                    _notyf.Error("Sorry, Kindly provide message to send");
                    return Json("");
                }
                
                var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco);
                var phoneNos = new List<string>();
                
                if(sms.Recipient == SMSRecipient.ActiveFarmers)
                {
                    var startDate = new DateTime(sms.PeriodEnding.Year, sms.PeriodEnding.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    var activeSnos = _context.ProductIntake
                        .Where(p => p.SaccoCode == sacco && p.TransDate >= startDate && p.TransDate <= endDate)
                        .Select(s => s.Sno).Distinct().ToList();
                    suppliers = suppliers.Where(s => activeSnos.Contains(s.Sno.ToString()));
                }

                if (sms.Recipient == SMSRecipient.SpecificLocation)
                    suppliers = suppliers.Where(s => s.Location.ToUpper().Equals(sms.Location.ToUpper()));

                if (sms.Recipient == SMSRecipient.SpecificTransporter)
                {
                    sms.Transporter = sms?.Transporter ?? "";
                    sms.Branch = sms?.Branch ?? "";
                    if (string.IsNullOrEmpty(sms.Transporter) && string.IsNullOrEmpty(sms.Branch))
                    {
                        _notyf.Error("Sorry, Kindly provide transporter");
                        return Json("");
                    }
                    var supplirNos = _context.DTransports.Where(t => t.saccocode == sacco && t.TransCode.ToUpper().Equals(sms.Transporter.ToUpper()))
                        .Select(t => t.Sno);
                    if(!string.IsNullOrEmpty(sms.Branch))
                        suppliers = suppliers.Where(s => s.Branch.ToUpper().Equals(sms.Branch.ToUpper())
                        && supplirNos.Contains(s.Sno));
                    if (!string.IsNullOrEmpty(sms.Transporter))
                        suppliers = suppliers.Where(s => supplirNos.Contains(s.Sno));
                }

                if (sms.Recipient == SMSRecipient.SharesCat)
                {
                    var category = _context.SharesCategories.FirstOrDefault(c => c.Name.ToUpper().Equals(sms.SharesCategory.ToUpper()));
                    if (category == null)
                    {
                        _notyf.Error("Sorry, Share category not found");
                        return Json("");
                    }
                    var shares = _context.DShares.Where(s => s.SaccoCode == sacco).ToList().GroupBy(s => s.Sno).ToList();
                    var catShareHolders = new List<string>();
                    shares.ForEach(s =>
                    {
                        var amount = s.Sum(a => a.Amount);
                        if (category.MinAmount >= amount && category.MaxAmount <= amount)
                            catShareHolders.Add(s.Key);
                    });
                    suppliers = suppliers.Where(s => catShareHolders.Contains(s.Sno.ToString()));
                }

                phoneNos = suppliers.Select(s => s.PhoneNo).Distinct().ToList();
                if (sms.Recipient == SMSRecipient.Individual)
                    phoneNos = new List<string>(sms.TelNo.Split(","));
                phoneNos.ForEach(t =>
                {
                    if (!string.IsNullOrEmpty(t))
                    {
                        var phone_first = t.Substring(0, 1);
                        if (phone_first == "0")
                            t = t.Substring(1);
                        var phone_three = t.Substring(0, 3);
                        if (phone_first == "254")
                            t = t.Substring(3);
                        var phone_four = t.Substring(0, 4);
                        if (phone_four == "+254")
                            t = t.Substring(4);

                        t = "254" + t;
                        _context.Messages.Add(new Message
                        {
                            Telephone = t,
                            Content = sms.Content,
                            ProcessTime = DateTime.UtcNow.AddHours(3).ToString(),
                            MsgType = "Outbox",
                            Replied = false,
                            DateReceived = DateTime.UtcNow.AddHours(3),
                            Source = loggedInUser,
                            Code = sacco
                        });
                    }
                });
                
                _context.SaveChanges();
                _notyf.Success("Message send successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }
    }
}
