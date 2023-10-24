using System;
using System.Collections.Generic;

namespace EasyPro.Models
{
	public class Refund
	{
		public Guid Id { get; set; }
		public string Ref { get; set; }
		public string Vendor { get; set; }
		public DateTime? BillDate { get; set; }
		public DateTime? DueDate { get; set; }
		public string Journal { get; set; }
		public string PaymentReference { get; set; }
		public string PaymentTerms { get; set; }
		public decimal? NetAmount { get; set; }
		public decimal? Tax { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Cost { get; set; }
		public decimal? Arrears { get; set; }
		public string Status { get; set; }
		public IEnumerable<RefundDetail> RefundDetails { get; set; }
		public IEnumerable<RefundJournal> RefundJournals { get; set; }
		public string ReceipientBank { get; set; }
		public string IncoTerm { get; set; }
		public string FiscalPosition { get; set; }
		public string Personnel { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public string SaccoCode { get; set; }
		public string Branch { get; set; }
	}
}
