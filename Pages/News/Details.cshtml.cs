using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using FakeNews.TFIDF;

namespace FakeNews.Pages.News
{
    [BindProperties]
    public class DetailsModel : PageModel
    {
        private readonly Data.FakeNewsContext _context;

        public DetailsModel(Data.FakeNewsContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.News News { get; set; }

        [BindProperty]
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

        public async Task<IActionResult> OnPostLikeTask()
        {
            _context.Attach(News).Entity.NumberOfLikes = _context.Attach(News).Entity.NumberOfLikes + 1;

            _context.SaveChangesAsync();

            Message = $"You liked this news";

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDislikeTask()
        {
            _context.Attach(News).Entity.NumberOfDislike = _context.Attach(News).Entity.NumberOfDislike + 1;

            _context.SaveChangesAsync();

            Message = $"You disliked this news";

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostCheckNewsTask(int id)
        {
            //var documents = new string[] { _context.News.FirstOrDefaultAsync(m => m.ID == id).Result.Content };
            //var tfidfResult = AlgorithmForTfidf.Transform(documents, 2);
            //tfidfResult = AlgorithmForTfidf.Normalize(tfidfResult);

            //for (var index = 0; index < tfidfResult.Length; index++)
            //{
            //    Console.WriteLine(documents[index]);

            //    foreach (var value in tfidfResult[index])
            //    {
            //        Console.Write(value + ", ");

            //        if (value < 0)
            //        {
            //            _context.Attach(News).Entity.Content = "Real";
            //        }
            //        else
            //        {
            //            _context.Attach(News).Entity.Content = "Fake";
            //        }
            //    }

            //    _context.SaveChangesAsync();

            //    Message = $"This news is" + _context.News.FirstOrDefaultAsync(m => m.ID == id).Result.Credibility;

            //    Console.WriteLine("\n");
            //}

            if (id == null)
            {
                return NotFound();
            }

            News = await _context.News.FindAsync(id);

            if (News != null)
            {
                var newsCredibility = Main.Tfidf(new string[]
                    {_context.News.FirstOrDefaultAsync(m => m.ID == id).Result.Content});

                _context.Attach(News).Entity.Credibility = newsCredibility;

                Message = $"This news is {newsCredibility}";
            }

            return RedirectToPage("./Index");
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.ID == id);
        }
    }
}
