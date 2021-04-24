using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FakeNews.Pages.News
{
    public class DetailsModel : PageModel
    {
        private readonly FakeNews.Data.FakeNewsContext _context;

        public DetailsModel(FakeNews.Data.FakeNewsContext context)
        {
            _context = context;
        }

        public Models.News News { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            News = await _context.News.FirstOrDefaultAsync(m => m.ID == id);

            if (News == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
