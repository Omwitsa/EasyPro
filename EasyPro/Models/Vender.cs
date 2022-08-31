using System;

namespace EasyPro.Models
{
	public class Vender
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Street1 { get; set; }
		public string Street2 { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string PhoneNo { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string WebSite { get; set; }
		public string SalesPerson { get; set; }
		public string PurchasePaymentTerms { get; set; }
		public string SalesPaymentTerms { get; set; }
		public string FiscalPosition { get; set; }
		public string Ref { get; set; }
		public string Industry { get; set; }
		public string APGlAccount { get; set; }
		public string Bank { get; set; }
		public string BankAccount { get; set; }
		public string Notes { get; set; }
		public bool? Closed { get; set; }
		public string Personnel { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
	}
}
