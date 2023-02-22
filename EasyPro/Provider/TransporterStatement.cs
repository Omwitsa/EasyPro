using EasyPro.Constants;
using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.ViewModels.FarmersVM;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyPro.Provider
{
    public class TransporterStatement : IStatement
    {
        private readonly MORINGAContext _context;
        public TransporterStatement(MORINGAContext context)
        {
            _context = context;
        }

        public dynamic GenerateStatement(StatementFilter filter)
        {
            filter.Code = filter.Code ?? "";
            filter.Branch = filter.Branch ?? "";

            var startDate = new DateTime(filter.Date.Year, filter.Date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var transporterFarmers = _context.DTransports.Where(t => t.TransCode.ToUpper().Equals(filter.Code.ToUpper()) 
                && t.saccocode == filter.Sacco && t.Branch.ToUpper().Equals(filter.Branch.ToUpper()))
                .Select(t => t.Sno).ToList();

            var intakes = _context.ProductIntake.Where(i => transporterFarmers.Contains(i.Sno) && i.SaccoCode == filter.Sacco
            && i.Branch.ToUpper().Equals(filter.Branch.ToUpper()) && i.TransDate >= startDate && i.TransDate <= endDate && i.CR > 0)
                .OrderBy(i => i.TransDate).ToList();

            var supplierGroupedIntakes = intakes.GroupBy(i => i.Sno).ToList();

            var transporters = new List<dynamic>();
            decimal totalKgs = 0;
            decimal? grossPay = 0;
            
            supplierGroupedIntakes.ForEach(i =>
            {
                var intake = i.FirstOrDefault();
                var transport = _context.DTransports.FirstOrDefault(t => t.Sno == intake.Sno
                && t.saccocode == filter.Sacco && t.Branch.ToUpper().Equals(filter.Branch.ToUpper()));
                var qty = i.Sum(p => p.Qsupplied);
                transporters.Add(new
                {
                    intake.Sno,
                    qnty = qty,
                    payable = qty * transport.Rate
                });
                totalKgs += qty;
                grossPay += (qty * transport.Rate);
            });

            var deductions = _context.ProductIntake.Where(i => i.Sno.ToUpper().Equals(filter.Code.ToUpper()) && i.SaccoCode == filter.Sacco
            && i.Branch.ToUpper().Equals(filter.Branch.ToUpper()) && i.TransDate >= startDate && i.TransDate <= endDate
            && i.DR > 0).OrderBy(i => i.TransDate).ToList();

            var totalDeductions = deductions.Sum(d => d.DR);

            var transporter = _context.DTransporters.FirstOrDefault(s => s.TransCode.ToUpper().Equals(filter.Code.ToUpper()) && s.ParentT == filter.Sacco && s.Tbranch == filter.Branch);
            var company = _context.DCompanies.FirstOrDefault(c => c.Name == filter.Sacco);
            return new
            {
                transporters,
                totalKgs,
                grossPay,
                deductions,
                totalDeductions,
                netPay = grossPay - totalDeductions,
                transporter,
                company
            };
        }
    }
}
