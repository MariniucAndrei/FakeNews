using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FakeNews.Pages.News
{
    public class DetailsModel : PageModel
    {
        private readonly Data.FakeNewsContext _context;

        public DetailsModel(Data.FakeNewsContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.News News { get; set; }

        public string Message { get; set; }

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

        public void OnPostLikeTask()
        {
            _context.Attach(News).Entity.NumberOfLikes = _context.Attach(News).Entity.NumberOfLikes + 1;

            _context.SaveChangesAsync();

            Message = $"Your liked this news";
        }

        public void OnPostDislikeTask()
        {
            _context.Attach(News).Entity.NumberOfDislike = _context.Attach(News).Entity.NumberOfDislike + 1;

            _context.SaveChangesAsync();

            Message = $"Your disliked this news";
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.ID == id);
        }
    }
}
