using EasyPro.Models;
using EasyPro.ViewModels.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Repository
{
    public class ReportingConcrete: IReporting
    {
        decimal totalkgs = 0;
        private readonly MORINGAContext _context;
        private readonly IConfiguration _configuration;
        public ReportingConcrete(MORINGAContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public List<UserMasterViewModel> GetUserwiseReport()
        {
            try
            {
                var listofusers = (from DSupplier in
                                       _context.DSuppliers.AsNoTracking()
                                   select new UserMasterViewModel()
                                  {
                                       Sno= DSupplier.Sno,
                                       Regdate = DSupplier.Regdate,
                                       IdNo = DSupplier.IdNo,
                                       Names = DSupplier.Names,
                                       AccNo = DSupplier.AccNo,
                                       Bcode = DSupplier.Bcode,
                                       Bbranch = DSupplier.Bbranch,
                                       Type = DSupplier.Type,
                                       Village = DSupplier.Village,
                                       Location = DSupplier.Location,
                                       Division = DSupplier.Division,
                                       District = DSupplier.District,
                                       County = DSupplier.County,
                                       Active = DSupplier.Active,
                                       PhoneNo = DSupplier.PhoneNo,
                                       Address= DSupplier.Address,
                                   }).ToList();
                return listofusers;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<UserIntakeViewModel> GetUserintakeReport()
        {
            try
            {
                
                var listofintakes = (from ProductIntake in
                                       _context.ProductIntake.Where(u=>u.Qsupplied>0).AsNoTracking()
                                   select new UserIntakeViewModel()
                                   {
                                       Sno = ProductIntake.Sno,
                                       TransDate = ProductIntake.TransDate,
                                       ProductType = ProductIntake.ProductType,
                                       Qsupplied = ProductIntake.Qsupplied,
                                       Ppu = ProductIntake.Ppu,
                                       CR = ProductIntake.CR,
                                       Balance = (ProductIntake.Qsupplied),

                                   }).ToList();
                return listofintakes;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<UserDeduViewModel> GetUserdeductionReport()
        {
            try
            {

                var listofded = (from ProductIntake in
                                       _context.ProductIntake.Where(u => u.Qsupplied == 0).AsNoTracking()
                                     select new UserDeduViewModel()
                                     {
                                         Sno = ProductIntake.Sno,
                                         TransDate = ProductIntake.TransDate,
                                         ProductType = ProductIntake.ProductType,
                                         Qsupplied = ProductIntake.Qsupplied,
                                         DR = ProductIntake.DR,
                                     }).ToList();
                return listofded;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
