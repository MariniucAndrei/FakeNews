using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeNews.Pages.News
{
    public class IndexModel : PageModel
    {
        private readonly Data.FakeNewsContext _context;

        public IndexModel(Data.FakeNewsContext context)
        {
            _context = context;
        }

        public IList<Models.News> News { get;set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public SelectList Domain { get; set; }

        [BindProperty(SupportsGet = true)]
        public string NewsDomain { get; set; }

        public SelectList Credibility { get; set; }

        [BindProperty(SupportsGet = true)]
        public string NewsCredibility { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<string> domainQuery = from n in _context.News orderby n.Domain select n.Domain;

            IQueryable<string> credibilityQuery = from n in _context.News orderby n.Credibility select n.Credibility;

            var news = from n in _context.News select n;

            if (!string.IsNullOrEmpty(SearchString))
            {
                news = news.Where(s => s.Title.Contains(SearchString) || s.Content.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(NewsDomain))
            {
                news = news.Where(x => x.Domain == NewsDomain);
            }

            if (!string.IsNullOrEmpty(NewsCredibility))
            {
                news = news.Where(x => x.Credibility == NewsCredibility);
            }

            Domain = new SelectList(await domainQuery.Distinct().ToListAsync());
            Credibility = new SelectList(await credibilityQuery.Distinct().ToListAsync());

            News = await news.OrderByDescending(x => x.PublishDate).ToListAsync();
        }
    }
}
