using bt1_api.Model;
using Microsoft.EntityFrameworkCore;
using web_api.Model;

namespace bt1_api.DBContext
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public DbSet<MasterProduct> MasterProduct { get; set; }
        public DbSet<SaleOut> SaleOut { get; set; }
        public DbSet<SaleOut> SaleOutDTO { get; set; }
    }
}
