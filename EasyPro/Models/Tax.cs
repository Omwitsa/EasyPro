using System;

namespace EasyPro.Models
{
	public class Tax
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Computation { get; set; }
		public string GlAccount { get; set; }
        public decimal? Rate { get; set; }
        public string Scope { get; set; }
		public bool Closed { get; set; }
		public string Personnel { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public string SaccoCode { get; set; }
	}
}
