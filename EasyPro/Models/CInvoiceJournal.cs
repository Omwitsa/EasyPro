using System;

namespace EasyPro.Models
{
	public class CInvoiceJournal
	{
		public Guid Id { get; set; }
		public Guid? CInvoiceId { get; set; }
		public string GlAccount { get; set; }
		public string Label { get; set; }
		public decimal? Debit { get; set; }
		public decimal? Credit { get; set; }
	}
}
