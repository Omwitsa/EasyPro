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
    public class SupplierStatement : IStatement
    {
        private readonly MORINGAContext _context;
        private readonly BosaDbContext _bosaDbContext;
        public SupplierStatement(MORINGAContext context, BosaDbContext bosaDbContext)
        {
            _context = context;
            _bosaDbContext = bosaDbContext;
        }

        public async Task<dynamic> GenerateStatement(StatementFilter filter)
        {
            filter.Code = filter.Code ?? "";
            filter.Branch = filter.Branch ?? "";

            var startDate = new DateTime(filter.Date.Year, filter.Date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var productIntakeslist = await _context.ProductIntake.Where(i => i.Sno.ToUpper().Equals(filter.Code.ToUpper()) && i.SaccoCode == filter.Sacco
            && i.TransDate >= startDate && i.TransDate <= endDate && i.Branch.ToUpper().Equals(filter.Branch.ToUpper()))
                .ToListAsync();

            var intakes = productIntakeslist.Where(i => (i.TransactionType == TransactionType.Intake || i.TransactionType == TransactionType.Correction))
                .OrderBy(i => i.TransDate).ToList();

            var dailyGroupedIntakes = intakes.GroupBy(i => i.TransDate).ToList();
            var supplies = new List<dynamic>();
            decimal totalKgs = 0;
            decimal? grossPay = 0;
            dailyGroupedIntakes.ForEach(i =>
            {
                var intake = i.FirstOrDefault();
                var price = intake.Ppu;
                var qty = i.Sum(p => p.Qsupplied);
                supplies.Add(new
                {
                    date = i.Key,
                    qnty = qty,
                    price,
                    payable = qty * price
                });
                totalKgs += qty;
                grossPay += (qty * price);
            });

            var deductionIntakes = productIntakeslist.Where(i => i.TransactionType == TransactionType.Deduction
            && (i.CR > 0 || i.DR > 0)).OrderBy(i => i.TransDate).ToList();
            var totalDeductions = deductionIntakes.Sum(d => d.DR) - deductionIntakes.Sum(d => d.CR);
            var deductions = new List<dynamic>();
            if (filter.Sacco != StrValues.Slopes)
            {
                var transportationDeductions = deductionIntakes.Where(i => i.Description == "Transport");
                deductions.Add(new
                {
                    TransDate = endDate,
                    Description = "Transport",
                    DR = transportationDeductions.Sum(i => i.DR)
                });
            }

            var otherDeductions = deductionIntakes.Where(i => i.Description != "Transport").ToList();
            otherDeductions.ForEach(i =>
            {
                i.DR = i.DR - i.CR;
                deductions.Add(new
                {
                    i.TransDate, 
                    i.Description,
                    i.DR
                });
            });

            var saccoLoans = new List<dynamic>();
            if (filter.Sacco == StrValues.Slopes)
            {
                var loans = await _bosaDbContext.LOANBAL.Where(l => l.MemberNo.ToUpper().Equals(filter.Code.ToUpper())
                && l.Balance > 0 && l.Companycode == StrValues.SlopesCode).ToListAsync();
                var types = await _bosaDbContext.LOANTYPE.Where(t => t.CompanyCode == StrValues.SlopesCode).ToListAsync();
                loans.ForEach(l =>
                {
                    var type = types.FirstOrDefault(t => t.LoanCode.ToUpper().Equals(l.LoanCode.ToUpper()));
                    saccoLoans.Add(new
                    {
                        l.Balance,
                        l.Installments,
                        l.LoanCode,
                        l.LoanNo,
                        type.LoanType
                    });
                });
            }
            var shares = await _context.DShares.Where(m => m.SaccoCode == filter.Sacco && m.Type.Contains("shares") 
            && m.Sno.ToUpper().Equals(filter.Code.ToUpper()) && m.Branch == filter.Branch)
                .ToListAsync();
            var sharesAmount = shares.Sum(x => x.Amount);
            var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno == filter.Code && s.Scode == filter.Sacco && s.Branch == filter.Branch);
            var company = _context.DCompanies.FirstOrDefault(c => c.Name == filter.Sacco);
            company.SupStatementNote = company?.SupStatementNote ?? "";

            var transport = _context.DTransports.FirstOrDefault(t => t.Sno.ToUpper().Equals(filter.Code.ToUpper()) && t.saccocode== filter.Sacco && t.Branch == filter.Branch);
            var transporterName = "";
            if (transport != null)
            {
                transport.TransCode = transport?.TransCode ?? "";
                var transporter = _context.DTransporters.FirstOrDefault(t => t.TransCode.ToUpper().Equals(transport.TransCode.ToUpper()) && t.ParentT == filter.Sacco && t.Tbranch == filter.Branch);
                transporterName = transporter?.TransName ?? "";
            }
            return new
            {
                supplies,
                totalKgs,
                grossPay,
                deductions,
                saccoLoans,
                totalDeductions,
                netPay = grossPay - totalDeductions,
                supplier,
                company,
                transporterName,
                sharesAmount
            };
        }

        public async Task<dynamic> GetTransporterFarmersStat(StatementFilter filter)
        {
            filter.Code = filter.Code ?? "";
            filter.Branch = filter.Branch ?? "";

            var startDate = new DateTime(filter.Date.Year, filter.Date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var suppliers = await _context.DSuppliers.Where(s => s.Scode == filter.Sacco && s.Branch == filter.Branch).ToListAsync();
            var transporter = await _context.DTransporters.FirstOrDefaultAsync(t => t.TransCode == filter.Code && t.ParentT == filter.Sacco && t.Tbranch == filter.Branch);
            if (StrValues.Slopes == filter.Sacco)
            {
                transporter = await _context.DTransporters.FirstOrDefaultAsync(t => t.CertNo == filter.Code && t.ParentT == filter.Sacco && t.Tbranch == filter.Branch);
                filter.Code = transporter?.TransCode ?? "";
            }

            var transporterSuppliers = await _context.DTransports.Where(s => s.TransCode == filter.Code && s.saccocode == filter.Sacco && s.Branch == filter.Branch)
                .Select(s => s.Sno.ToUpper()).ToListAsync();

            var productIntakeslist = await _context.ProductIntake.Where(i => transporterSuppliers.Contains(i.Sno.ToUpper()) && i.SaccoCode == filter.Sacco
            && i.TransDate >= startDate && i.TransDate <= endDate && i.Branch.ToUpper().Equals(filter.Branch.ToUpper()))
                .ToListAsync();

            var intakes = productIntakeslist.Where(i => (i.TransactionType == TransactionType.Intake || i.TransactionType == TransactionType.Correction))
                .OrderBy(i => i.TransDate).ToList();

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == filter.Sacco);
            company.SupStatementNote = company?.SupStatementNote ?? "";

            var supplierGroupedIntakes = intakes.GroupBy(i => i.Sno).ToList();
            var transpoterIntakes = new List<dynamic>();
            supplierGroupedIntakes.ForEach(s =>
            {
                var intake = s.FirstOrDefault();
                var price = intake.Ppu;
                var qty = s.Sum(p => p.Qsupplied);
                transpoterIntakes.Add(new
                {
                    supplier = suppliers.FirstOrDefault(o => o.Sno == s.Key),
                    supplies = s.ToList(),
                    qnty = qty,
                });
            });

            return new
            {
                transpoterIntakes,
                company,
                transporter
            };
        }
    }
}
