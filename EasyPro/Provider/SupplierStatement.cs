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

namespace EasyPro.Provider
{
    public class SupplierStatement : IStatement
    {
        private readonly MORINGAContext _context;
        public SupplierStatement(MORINGAContext context)
        {
            _context = context;
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
                //var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode == filter.Sacco && p.Products.ToUpper().Equals(intake.ProductType.ToUpper()));
                var qty = i.Sum(p => p.Qsupplied);
                supplies.Add(new
                {
                    date = i.Key,
                    qnty = qty,
                    price = price,
                   //price.Price,
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
                totalDeductions,
                netPay = grossPay - totalDeductions,
                supplier,
                company,
                transporterName
            };
        }
    }
}
