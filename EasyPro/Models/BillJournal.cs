using System;

namespace EasyPro.Models
{
	public class BillJournal
	{
		public Guid Id { get; set; }
		public Guid? BillId { get; set; }
		public string GlAccount { get; set; }
		public string Label { get; set; }
		public decimal? Debit { get; set; }
		public decimal? Credit { get; set; }
	}
}
