﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;

namespace EasyPro.Controllers
{
    public class UsergroupsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public UsergroupsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Usergroups
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.Usergroups.Where(g => g.SaccoCode == sacco).ToListAsync());
        }

        // GET: Usergroups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var usergroup = await _context.Usergroups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (usergroup == null)
            {
                return NotFound();
            }

            return View(usergroup);
        }

        // GET: Usergroups/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: Usergroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Usergroup usergroup)
        {
            utilities.SetUpPrivileges(this);
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                if (string.IsNullOrEmpty(usergroup.GroupId))
                {
                    _notyf.Error("Sorry, Kindly provide group code");
                    return View(usergroup);
                }
                if (string.IsNullOrEmpty(usergroup.GroupName))
                {
                    _notyf.Error("Sorry, Kindly provide group name");
                    return View(usergroup);
                }
                if (_context.Usergroups.Any(g => g.GroupId.ToUpper().Equals(usergroup.GroupId.ToUpper()) && g.SaccoCode.ToUpper().Equals(sacco.ToUpper())))
                {
                    _notyf.Error("Sorry, Group code already exist");
                    return View(usergroup);
                }
                if (_context.Usergroups.Any(g => g.GroupName.ToUpper().Equals(usergroup.GroupName.ToUpper()) && g.SaccoCode.ToUpper().Equals(sacco.ToUpper())))
                {
                    _notyf.Error("Sorry, Group name already exist");
                    return View(usergroup);
                }

                usergroup.SaccoCode = sacco;
                usergroup.Registration = usergroup?.Registration ?? false;
                usergroup.Activity = usergroup?.Activity ?? false;
                usergroup.Reports = usergroup?.Reports ?? false;
                usergroup.Setup = usergroup?.Setup ?? false;
                usergroup.Files = usergroup?.Files ?? false;
                usergroup.Accounts = usergroup?.Accounts ?? false;
                usergroup.Deductions = usergroup?.Deductions ?? false;
                usergroup.Staff = usergroup?.Staff ?? false;
                usergroup.Store = usergroup?.Store ?? false;
                usergroup.Flmd = usergroup?.Flmd ?? false;
                usergroup.SaccoReports = usergroup?.SaccoReports ?? false;
                usergroup.RFarmers = usergroup?.RFarmers ?? false;
                usergroup.RTransporter = usergroup?.RTransporter ?? false;
                usergroup.RImportS = usergroup?.RImportS ?? false;
                usergroup.RVendor = usergroup?.RVendor ?? false;
                usergroup.RCustomer = usergroup?.RCustomer ?? false;
                usergroup.StandingOrder = usergroup?.StandingOrder ?? false;
                usergroup.CashShares = usergroup?.CashShares ?? false;
                usergroup.DefaultDed = usergroup?.DefaultDed ?? false;
                usergroup.DedFarmer = usergroup?.DedFarmer ?? false;
                usergroup.DedTransport = usergroup?.DedTransport ?? false;
                usergroup.DedStaff = usergroup?.DedStaff ?? false;
                usergroup.TransporterAssign = usergroup?.TransporterAssign ?? false;
                usergroup.Millers = usergroup?.Millers ?? false;
                usergroup.Marketers = usergroup?.Marketers ?? false;
                usergroup.Pulping = usergroup?.Pulping ?? false;
                usergroup.Milling = usergroup?.Milling ?? false;
                usergroup.Marketing = usergroup?.Marketing ?? false;
                usergroup.CofPricing = usergroup?.CofPricing ?? false;
                usergroup.CofPayroll = usergroup?.CofPayroll ?? false;
                usergroup.SetProducts = usergroup?.SetProducts ?? false;
                usergroup.SetPrice = usergroup?.SetPrice ?? false;
                usergroup.SetFarmersDif = usergroup?.SetFarmersDif ?? false;
                usergroup.SetOrganization = usergroup?.SetOrganization ?? false;
                usergroup.SetOrgBranch = usergroup?.SetOrgBranch ?? false;
                usergroup.SetUsers = usergroup?.SetUsers ?? false;
                usergroup.SetUserGroups = usergroup?.SetUserGroups ?? false;
                usergroup.SetCounties = usergroup?.SetCounties ?? false;
                usergroup.SetSubCounties = usergroup?.SetSubCounties ?? false;
                usergroup.SetWards = usergroup?.SetWards ?? false;
                usergroup.SetLocation = usergroup?.SetLocation ?? false;
                usergroup.SetDedTypes = usergroup?.SetDedTypes ?? false;
                usergroup.SetBanks = usergroup?.SetBanks ?? false;
                usergroup.SetBanksBranch = usergroup?.SetBanksBranch ?? false;
                usergroup.SetZones = usergroup?.SetZones ?? false;
                usergroup.SetDebtors = usergroup?.SetDebtors ?? false;
                usergroup.SetTaxes = usergroup?.SetTaxes ?? false; 
                usergroup.SetResetPass = usergroup?.SetResetPass ?? false;
                usergroup.SetSharesCat = usergroup?.SetSharesCat ?? false;
                usergroup.SetRoutes = usergroup?.SetRoutes ?? false;
                usergroup.StoreProducts = usergroup?.StoreProducts ?? false;
                usergroup.ProdSupplier = usergroup?.ProdSupplier ?? false;
                usergroup.ProdSales = usergroup?.ProdSales ?? false;
                usergroup.SalesReturn = usergroup?.SalesReturn ?? false;
                usergroup.ProdDispatch = usergroup?.ProdDispatch ?? false;
                usergroup.ProdIntake = usergroup?.ProdIntake ?? false;
                usergroup.IntakeCorrection = usergroup?.IntakeCorrection ?? false;
                usergroup.ImportIntake = usergroup?.ImportIntake ?? false;
                usergroup.MilkTest = usergroup?.MilkTest ?? false;
                usergroup.VarBalancing = usergroup?.VarBalancing ?? false;
                usergroup.Dispatch = usergroup?.Dispatch ?? false;
                usergroup.SendSms = usergroup?.SendSms ?? false;
                usergroup.MilkControl = usergroup?.MilkControl ?? false;
                usergroup.BranchMilkEnquiry = usergroup?.BranchMilkEnquiry ?? false;
                usergroup.SupplierStatement = usergroup?.SupplierStatement ?? false;
                usergroup.TransporterStatement = usergroup?.TransporterStatement ?? false;
                usergroup.ChartsofAcc = usergroup?.ChartsofAcc ?? false;
                usergroup.JournalPosting = usergroup?.JournalPosting ?? false;
                usergroup.Glinquiry = usergroup?.Glinquiry ?? false;
                usergroup.Budgettings = usergroup?.Budgettings ?? false;
                usergroup.JournalListing = usergroup?.JournalListing ?? false;
                usergroup.TrialBalance = usergroup?.TrialBalance ?? false;
                usergroup.IncomeStatement = usergroup?.IncomeStatement ?? false;
                usergroup.BalanceSheet = usergroup?.BalanceSheet ?? false;
                usergroup.Payroll = usergroup?.Payroll ?? false;
                usergroup.Bills = usergroup?.Bills ?? false;
                usergroup.Refunds = usergroup?.Refunds ?? false;
                usergroup.CustomerInvoices = usergroup?.CustomerInvoices ?? false;
                usergroup.CreditNotes = usergroup?.CreditNotes ?? false;
                usergroup.VendorProducts = usergroup?.VendorProducts ?? false;
                usergroup.CustomerProducts = usergroup?.CustomerProducts ?? false;

                _context.Add(usergroup);
                await _context.SaveChangesAsync();
                _notyf.Success("Group saved successfuly");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
                return View(usergroup);
            }
        }

        // GET: Usergroups/Edit/5
        public async Task<IActionResult> Edit(int? Id)
        {
            utilities.SetUpPrivileges(this);
            if (Id == null)
            {
                return NotFound();
            }
            
            var usergroup = await _context.Usergroups.AsNoTracking().FirstOrDefaultAsync(u=>u.Id==Id);
            if (usergroup == null)
            {
                return NotFound();
            }
            return View(usergroup);
        }

        // POST: Usergroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long Id,  Usergroup usergroup)
        {
            utilities.SetUpPrivileges(this);
            if (Id != usergroup.Id)
            {
                return NotFound();
            }
           

            if (ModelState.IsValid)
            {
                try
                {
                    var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                    var role = HttpContext.Session.GetString(StrValues.UserGroup) ?? "";

                    if (role.Equals(usergroup.GroupName))
                    {
                        _notyf.Error("Permission Denied");
                            return View(usergroup);
                    }

                    //if (string.IsNullOrEmpty(usergroup.GroupId))
                    //{
                    //    _notyf.Error("Sorry, Kindly provide group code");
                    //    return View(usergroup);
                    //}
                    //if (string.IsNullOrEmpty(usergroup.GroupName))
                    //{
                    //    _notyf.Error("Sorry, Kindly provide group name");
                    //    return View(usergroup);
                    //}
                    //var codeExist = _context.Usergroups.AsNoTracking().Any(g => g.GroupId.ToUpper().Equals(usergroup.GroupId.ToUpper())
                    //&& !g.GroupId.ToUpper().Equals(usergroup.GroupId.ToUpper()) && g.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
                    //if (codeExist)
                    //{
                    //    _notyf.Error("Sorry, Group code already exist");
                    //    return View(usergroup);
                    //}
                    //var nameExist = _context.Usergroups.AsNoTracking().Any(g => g.GroupName.ToUpper().Equals(usergroup.GroupName.ToUpper())
                    //&& !g.GroupId.ToUpper().Equals(usergroup.GroupId.ToUpper()) && g.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
                    //if (nameExist)
                    //{
                    //    _notyf.Error("Sorry, Group name already exist");
                    //    return View(usergroup);
                    //}

                    usergroup.SaccoCode = sacco;
                    usergroup.Registration = usergroup.Registration;
                    usergroup.Activity = usergroup.Activity;
                    usergroup.Reports = usergroup.Reports;
                    usergroup.Setup = usergroup.Setup;
                    usergroup.Files = usergroup.Files ;
                    usergroup.Accounts = usergroup.Accounts;
                    usergroup.Deductions = usergroup.Deductions;
                    usergroup.Staff = usergroup.Staff;
                    usergroup.Store = usergroup.Store;
                    usergroup.Flmd = usergroup.Flmd;
                    usergroup.SaccoReports = usergroup.SaccoReports;
                    usergroup.RFarmers = usergroup.RFarmers;
                    usergroup.RTransporter = usergroup.RTransporter;
                    usergroup.RImportS = usergroup.RImportS;
                    usergroup.RVendor = usergroup.RVendor;
                    usergroup.RCustomer = usergroup.RCustomer;
                    usergroup.StandingOrder = usergroup.StandingOrder;
                    usergroup.CashShares = usergroup.CashShares;
                    usergroup.DefaultDed = usergroup.DefaultDed ;
                    usergroup.DedFarmer = usergroup.DedFarmer;
                    usergroup.DedTransport = usergroup.DedTransport;
                    usergroup.DedStaff = usergroup.DedStaff;
                    usergroup.TransporterAssign = usergroup.TransporterAssign;
                    usergroup.Millers = usergroup.Millers;
                    usergroup.Marketers = usergroup.Marketers;
                    usergroup.Pulping = usergroup.Pulping;
                    usergroup.Milling = usergroup.Milling;
                    usergroup.Marketing = usergroup.Marketing;
                    usergroup.CofPricing = usergroup.CofPricing;
                    usergroup.CofPayroll = usergroup.CofPayroll;
                    usergroup.SetProducts = usergroup.SetProducts;
                    usergroup.SetPrice = usergroup.SetPrice;
                    usergroup.SetFarmersDif = usergroup.SetFarmersDif;
                    usergroup.SetOrganization = usergroup.SetOrganization;
                    usergroup.SetOrgBranch = usergroup.SetOrgBranch;
                    usergroup.SetUsers = usergroup.SetUsers;
                    usergroup.SetUserGroups = usergroup.SetUserGroups;
                    usergroup.SetCounties = usergroup.SetCounties;
                    usergroup.SetSubCounties = usergroup.SetSubCounties;
                    usergroup.SetWards = usergroup.SetWards;
                    usergroup.SetLocation = usergroup.SetLocation;
                    usergroup.SetDedTypes = usergroup.SetDedTypes;
                    usergroup.SetBanks = usergroup.SetBanks;
                    usergroup.SetBanksBranch = usergroup.SetBanksBranch;
                    usergroup.SetZones = usergroup.SetZones;
                    usergroup.SetDebtors = usergroup.SetDebtors;
                    usergroup.SetTaxes = usergroup.SetTaxes;
                    usergroup.SetSharesCat = usergroup.SetSharesCat;
                    usergroup.SetRoutes = usergroup.SetRoutes;
                    usergroup.StoreProducts = usergroup.StoreProducts;
                    usergroup.ProdSupplier = usergroup.ProdSupplier;
                    usergroup.ProdSales = usergroup.ProdSales;
                    usergroup.SalesReturn = usergroup.SalesReturn;
                    usergroup.ProdDispatch = usergroup.ProdDispatch;
                    usergroup.ProdIntake = usergroup.ProdIntake;
                    usergroup.IntakeCorrection = usergroup.IntakeCorrection;
                    usergroup.ImportIntake = usergroup.ImportIntake;
                    usergroup.MilkTest = usergroup.MilkTest;
                    usergroup.VarBalancing = usergroup.VarBalancing;
                    usergroup.Dispatch = usergroup.Dispatch;
                    usergroup.SendSms = usergroup.SendSms;
                    usergroup.MilkControl = usergroup.MilkControl;
                    usergroup.BranchMilkEnquiry = usergroup.BranchMilkEnquiry;
                    usergroup.SupplierStatement = usergroup.SupplierStatement;
                    usergroup.TransporterStatement = usergroup.TransporterStatement;
                    usergroup.ChartsofAcc = usergroup.ChartsofAcc;
                    usergroup.JournalPosting = usergroup.JournalPosting;
                    usergroup.Glinquiry = usergroup.Glinquiry;
                    usergroup.Budgettings = usergroup.Budgettings;
                    usergroup.JournalListing = usergroup.JournalListing;
                    usergroup.TrialBalance = usergroup.TrialBalance;
                    usergroup.IncomeStatement = usergroup.IncomeStatement;
                    usergroup.BalanceSheet = usergroup.BalanceSheet;
                    usergroup.Payroll = usergroup.Payroll;
                    usergroup.Bills = usergroup.Bills;
                    usergroup.Refunds = usergroup.Refunds;
                    usergroup.CustomerInvoices = usergroup.CustomerInvoices;
                    usergroup.CreditNotes = usergroup.CreditNotes;
                    usergroup.VendorProducts = usergroup.VendorProducts;
                    usergroup.CustomerProducts = usergroup.CustomerProducts;
                    _context.Update(usergroup);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Group edited successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    _notyf.Error("Sorry, An error occurred");
                    if (!UsergroupExists(usergroup.GroupId))
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
            _notyf.Error("Sorry, An error occurred");
            return View(usergroup);
        }

        // GET: Usergroups/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var usergroup = await _context.Usergroups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (usergroup == null)
            {
                return NotFound();
            }

            return View(usergroup);
        }

        // POST: Usergroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            utilities.SetUpPrivileges(this);
            var usergroup = await _context.Usergroups.FindAsync(id);
            _context.Usergroups.Remove(usergroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsergroupExists(string id)
        {
            return _context.Usergroups.Any(e => e.GroupId == id);
        }
    }
}
