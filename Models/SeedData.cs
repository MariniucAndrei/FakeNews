using FakeNews.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FakeNews.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context =
                new FakeNewsContext(serviceProvider.GetRequiredService<DbContextOptions<FakeNewsContext>>()))
            {
                //Search for any news
                if (context.News.Any())
                {
                    return;
                }

                context.News.AddRange
                (
                    new News
                    {
                        Title = "Title1",
                        Content = "Content1",
                        PublishDate = DateTime.Today,
                        Domain = "medicine",
                        Credibility = "Real"
                    },
                    new News
                    {
                        Title = "Title2",
                        Content = "Content2",
                        PublishDate = DateTime.Today,
                        Domain = "sport",
                        Credibility = "Real"
                    },
                    new News
                    {
                        Title = "Title3",
                        Content = "Content3",
                        PublishDate = DateTime.Today,
                        Domain = "economics",
                        Credibility = "Real"
                    },
                    new News
                    {
                        Title = "Title4",
                        Content = "Content4",
                        PublishDate = DateTime.Today,
                        Domain = "lifestyle",
                        Credibility = "Fake"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
