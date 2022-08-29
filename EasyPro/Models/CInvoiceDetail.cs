using System;

namespace EasyPro.Models
{
	public class CInvoiceDetail
	{
		public Guid Id { get; set; }
		public Guid? CInvoiceId { get; set; }
		public string Product { get; set; }
		public string Label { get; set; }
		public string GlAccount { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Price { get; set; }
		public string Tax { get; set; }
		public decimal? SubTotal { get; set; }
	}
}
