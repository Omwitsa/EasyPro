using Microsoft.EntityFrameworkCore;
using static FastReport.Barcode.Iban;

namespace EasyPro.Models.BosaModels
{
    public class BosaDbContext : DbContext
    {
        public BosaDbContext()
        {
        }

        public BosaDbContext(DbContextOptions<BosaDbContext> options)
        : base(options)
        {
        }

        public virtual DbSet<Member> MEMBERS { get; set; }
        public virtual DbSet<SContrib> CONTRIB { get; set; }
        public virtual DbSet<LoanTypes> LOANTYPE { get; set; }
        public virtual DbSet<LoanBal> LOANBAL { get; set; }
        public virtual DbSet<ContribStandingOrder> CONTRIB_standingOrder { get; set; }
    }
}
