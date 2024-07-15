using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MVCTest.Models;

namespace MVCApplication.Areas.Blog.Controllers
{
    [Area("Blog")]
    public class ViewPostController : Controller
    {
        private readonly ILogger<ViewPostController> logger;
        private readonly AppDbContext _context;
        public ViewPostController(ILogger<ViewPostController> _logger, AppDbContext context)
        {
            logger = _logger;
            _context = context;
        }

        [Route("/post/{categoryslug?}")]
        public async Task<IActionResult> Index(string? categoryslug, int? page)
        {
            var categories = GetCategories();
            ViewBag.Categories = categories;
            ViewBag.categoryslug = categoryslug;

            Category category = null;

            if (!string.IsNullOrEmpty(categoryslug))
            {
                category = await _context.Categories.Where(c => c.Slug == categoryslug)
                                                  .Include(c => c.ChildrenCategories)
                                                  .FirstOrDefaultAsync();

                if (category == null)
                {
                    return NotFound();
                }
            }

            var posts = _context.Posts.
                        Include(p => p.Author).
                        Include(p => p.PostCategories).
                        ThenInclude(p => p.cate).
                        AsQueryable();

            posts.OrderBy(p => p.DateUpdated);

            var parentIds = new List<int>();

            if (category != null)
            {
                var ids = new List<int>();
                category.CategoryChildrenIds(category.ChildrenCategories, ids);
                ids.Add(category.Id);

                posts = posts.Where(p => p.PostCategories.Where(pc => ids.Contains(pc.CategoryId)).Any());

                category.ParentCategoryIds(category, parentIds);
            }

            List<Category> parentCates = new List<Category>();
            if (parentIds.Count > 0)
            {
               parentIds.RemoveAt(0);
                for (int i = parentIds.Count() - 1;i>=0;i--)
                {
                    parentCates.Add(_context.Categories.Where(c => c.Id == parentIds[i]).FirstOrDefault());
                }
            }

            ViewBag.parentCates = parentCates;

            ViewBag.Category = category;
            return View(posts.ToList());
        }

        [Route("/post/{postslug}.html")]
        public IActionResult Detail(string postslug)
        {
            return View();
        }
        private List<Category> GetCategories()
        {
            var categories = _context.Categories.Include(c => c.ChildrenCategories)
                                                .AsEnumerable()
                                                .Where(c => c.ParentCategory == null)
                                                .ToList();
            return categories;
        }
    }
}
