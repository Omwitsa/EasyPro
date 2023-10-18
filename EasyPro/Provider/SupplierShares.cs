using DinkToPdf.Contracts;
using DinkToPdf;
using EasyPro.IProvider;
using EasyPro.Models;
using System;
using System.Linq;
using EasyPro.Constants;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using EasyPro.ViewModels.FarmersVM;
using EasyPro.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Stripe;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EasyPro.Models.BosaModels;

namespace EasyPro.Provider
{
    public class SupplierShares
    {
        private readonly MORINGAContext _context;
        private readonly BosaDbContext _bosaDbContext;
        public SupplierShares(MORINGAContext context, BosaDbContext bosaDbContext)
        {
            _context = context;
            _bosaDbContext = bosaDbContext;
        }
        public async Task deductshares(SharesFilter filter, bool wherefrom)
        {
            //var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            //var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            //var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            IQueryable<DPreSet> dPreSets = _context.d_PreSets;
            IQueryable<DShare> dShares = _context.DShares;
            IQueryable<DSupplier> dSuppliers = _context.DSuppliers;
            var checkrecords = dPreSets.FirstOrDefault(e => e.Sno.ToUpper().Equals(filter.Code.ToUpper()) && e.saccocode == filter.Sacco
                && e.BranchCode == filter.Branch && e.Deduction.ToUpper().Equals("SHARES"));
            if (wherefrom)
            {
               if (filter.shares)
                  filter.shares = false;
                else
                   filter.shares = true;
                
                if (checkrecords != null)
                {
                    checkrecords.Stopped = filter.shares;
                    checkrecords.Auditdatetime = DateTime.Now;
                    checkrecords.AuditId = filter.LoggedInUser;
                    checkrecords.BranchCode = filter.Branch;
                    checkrecords.saccocode = filter.Sacco;
                }
                else
                {
                    var productIntake = new DPreSet
                    {
                        Sno = filter.Code,
                        Deduction = "SHARES",
                        Remark = "SHARES",
                        StartDate = DateTime.Today,
                        Rate = 1,
                        Stopped = filter.shares,
                        Auditdatetime = DateTime.Now,
                        AuditId = filter.LoggedInUser,
                        Rated = false,
                        BranchCode = filter.Branch,
                        saccocode = filter.Sacco
                    };
                    _context.d_PreSets.Add(productIntake);
                }
            }
            else
            {
                var getsupplier = dSuppliers.FirstOrDefault(m => m.Sno.ToUpper().Equals(filter.Code.ToUpper()) && m.Scode == filter.Sacco
                && m.Branch == filter.Branch);
                if(getsupplier != null)
                {
                    if (checkrecords != null)
                    {
                        var checkifexceedmaxshares = dShares.Where(m => m.SaccoCode == filter.Sacco && m.Branch == filter.Branch
                            && m.Type.Contains("shares") && m.Sno.ToUpper().Equals(filter.Code.ToUpper())).ToList();
                        if (checkifexceedmaxshares.Sum(x => x.Amount) >= 20000)
                        {
                            checkrecords.Stopped = true;
                            checkrecords.Auditdatetime = DateTime.Now;
                            checkrecords.AuditId = filter.LoggedInUser;
                            checkrecords.BranchCode = filter.Branch;
                            checkrecords.saccocode = filter.Sacco;

                            getsupplier.Shares = false;
                        }
                    }
                }
            }
            
        }
    }
}
