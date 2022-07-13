using EasyPro.Constants;
using EasyPro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Utils
{
    public class Utilities
    {
        private MORINGAContext _context;
        public Utilities(MORINGAContext context)
        {
            _context = context;
        }

        public void SetUpPrivileges(Controller controller)
        {
            var group = controller.HttpContext.Session.GetString(StrValues.UserGroup) ?? "";
            var usergroup = _context.Usergroups.FirstOrDefault(u => u.GroupName.Equals(group));
            controller.ViewBag.sacco = controller.HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            controller.ViewBag.filesRole = usergroup.Files;
            controller.ViewBag.accountsRole = usergroup.Accounts;
            controller.ViewBag.transactionsRole = usergroup.Transactions;
            controller.ViewBag.activityRole = usergroup.Activity;
            controller.ViewBag.setupRole = usergroup.Setup;
            controller.ViewBag.reportsRole = usergroup.Reports;
            controller.ViewBag.storeRole = usergroup.Setup;
        }
    }
}
