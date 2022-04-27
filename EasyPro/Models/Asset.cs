using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Asset
    {
        public int Assetsid { get; set; }
        public string AssetsNo { get; set; }
        public string AssetserialNo { get; set; }
        public string AssetsName { get; set; }
        public string AssetType { get; set; }
        public DateTime? Datebought { get; set; }
        public int? UnitNo { get; set; }
        public decimal? PurchasePrice { get; set; }
        public double? Depreciation { get; set; }
        public decimal? Currentvalue { get; set; }
        public string Notes { get; set; }
        public DateTime? Transdate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public decimal? Accdepreciation { get; set; }
        public decimal? Revaluation { get; set; }
        public decimal? Nrvbf { get; set; }
        public bool? Status { get; set; }
        public DateTime? Disposaldate { get; set; }
        public decimal? DAmount { get; set; }
        public string Accno { get; set; }
    }
}
