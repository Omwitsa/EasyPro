using System;

namespace EasyPro.Models
{
	public class CProduct
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Category { get; set; }
		public string Ref { get; set; }
		public string BarCode { get; set; }
		public decimal? Price { get; set; }
		public string CustomerTax { get; set; }
		public decimal? Cost { get; set; }
		public string Notes { get; set; }
		public string APGlAccount { get; set; }
		public bool Closed { get; set; }
		public string Personnel { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public string SaccoCode { get; set; }
	}
}
