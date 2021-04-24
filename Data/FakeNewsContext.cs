using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FakeNews.Models;

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
