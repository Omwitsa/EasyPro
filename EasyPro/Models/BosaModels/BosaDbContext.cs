using Microsoft.EntityFrameworkCore;

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
    }
}
