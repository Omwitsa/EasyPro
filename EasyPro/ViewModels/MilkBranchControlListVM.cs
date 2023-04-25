using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class MilkBranchControlListVM
    {
        public IEnumerable<MilkBranchControlList> milkBranchControlLists { get; set; }
        public decimal? IntakeTotal { get; set; }
        public decimal? SQuantityTotal { get; set; }
        public decimal? Total { get; set; }
    }
    public class MilkBranchControlList
    {
        public long Id { get; set; }
        public decimal? Intake { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? transdate { get; set; }
        public decimal? SQuantity { get; set; }
        public decimal? Reject { get; set; }
        public decimal? cfa { get; set; }
        public decimal? Spillage { get; set; }
        public decimal? Bf { get; set; }
        public string auditid { get; set; }
        public decimal? FromStation { get; set; }
        public decimal? Tostation { get; set; }
        public string code { get; set; }
        public string Branch { get; set; }
        public decimal Total { get; set; }
        public decimal Varriance { get; set; }
    }
    public class MilkEnqury
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? TransDate { get; set; }
        public string ProductType { get; set; }
        public decimal? Qsupplied { get; set; }
        public decimal? CR { get; set; }
        public decimal? DR { get; set; }
        public decimal? Balance { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public DateTime Auditdatetime { get; set; }
        public decimal? getsumkgs { get; set; }
    }
}
