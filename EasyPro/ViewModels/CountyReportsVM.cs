using EasyPro.Constants;
using EasyPro.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class CountyReportsVM
    {
        public IEnumerable<ProductIntakelist> ProductIntakelists { get; set; }
        public decimal? IntakeTotal { get; set; }
        public decimal? SQuantityTotal { get; set; }
        public decimal? Total { get; set; }
        public decimal? Expense { get; set; }
        public decimal? Income { get; set; }
        public DateTime date2 { get; set; }
        public string sacco { get; set; }

    }
    public class ProductIntakelist
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public decimal? Amount { get; set; }
        
    }
}
