using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EasyPro.Controllers.Coffee
{
    public class CustomerController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public CustomerController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            
            return View(_context.Customer);
            
        }
        
        public JsonResult InsertCustomers(List<Customer> customers)
        {

            using (MORINGAContext entities = new MORINGAContext())
            {

                //Check for NULL.
                if (customers == null)
                {
                    customers = new List<Customer>();
                }

                //Loop and insert records.
                foreach (Customer customer in customers)
                {
                    _context.Customers.Add(customer);
                }
                int insertedRecords = _context.SaveChanges();
                return Json(insertedRecords);
            }
            
        
    }
}
}
