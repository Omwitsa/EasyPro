using System;

using System.Collections.Generic;

namespace EasyPro.Models
{
	public class Bill
	{
		public Guid Id { get; set; }
		public string Vender { get; set; }
		public string Ref { get; set; }
		public string Remarks { get; set; }
		public DateTime? Date { get; set; }
		public DateTime? DueDate { get; set; }
		public string Journal { get; set; }
		public string PaymentReference { get; set; }
		public decimal? NetAmount { get; set; }
		public decimal? Tax { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? Arrears { get; set; }
		public string Status { get; set; }
		public IEnumerable<BillDetail> BillDetails { get; set; }
		public IEnumerable<BillJournal> BillJournals { get; set; }
		public string RecipientBank { get; set; }
		public string IncoTerm { get; set; }
		public string FiscalPosition { get; set; }
		public string Personnel { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
	}
}
