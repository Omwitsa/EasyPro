using System;

namespace EasyPro.Models
{
	public class RefundJournal
	{
		public Guid Id { get; set; }
		public Guid? RefundId { get; set; }
		public string GlAccount { get; set; }
		public string Label { get; set; }
		public decimal? Debit { get; set; }
		public decimal? Credit { get; set; }
	}
}
