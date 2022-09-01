using System;

namespace EasyPro.Models
{
	public class Customer
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Street1 { get; set; }
		public string Street2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Country { get; set; }
		public string PhoneNo { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string WebSite { get; set; }
		public string SalesPerson { get; set; }
		public string PurchasePaymentTerms { get; set; }
		public string SalesPaymentTerms { get; set; }
		public string FiscalPosition { get; set; }
		public string ARGlAccount { get; set; }
		public string Bank { get; set; }
		public string Tags { get; set; }
		public string Notes { get; set; }
		public bool Closed { get; set; }
		public string Personnel { get; set; }
		public string Reference { get; set; }
		public string industry { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public string SaccoCode { get; set; }
	}
}
