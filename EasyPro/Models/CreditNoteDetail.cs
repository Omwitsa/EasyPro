using System;

namespace EasyPro.Models
{
	public class CreditNoteDetail
	{
		public Guid Id { get; set; }
		public Guid? CreditNoteId { get; set; }
		public string Product { get; set; }
		public string Lable { get; set; }
		public string GlAccount { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Price { get; set; }
		public string Tax { get; set; }
		public decimal? SubTotal { get; set; }
	}
}
