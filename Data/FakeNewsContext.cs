using Microsoft.EntityFrameworkCore;

namespace FakeNews.Data
{
    public class FakeNewsContext : DbContext
    {
        public FakeNewsContext (DbContextOptions<FakeNewsContext> options)
            : base(options)
        {
        }

        public DbSet<FakeNews.Models.News> News { get; set; }
    }
}
