using App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCTest.Models;
using MVCTest.Models.Product;

namespace MVCTest.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize(Roles = RoleName.Administrator)]
    public class CategoryProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryProductController> _logger;

        [TempData]
        public string StatusMessage { get; set; }

        public CategoryProductController(AppDbContext context, ILogger<CategoryProductController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var qr = (from c in _context.CategoryProducts select c)
                     .Include(c => c.ParentCategory)
                     .Include(c => c.ChildrenCategories);

            var categories = (await qr.ToListAsync()).Where(c => c.ParentCategory == null).ToList();

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            var categories = await (from c in _context.CategoryProducts select c).ToListAsync();

            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục"
            });

            var selectList = new SelectList(categories, "Id", "Title", -1);

            ViewData["ParentCategoryId"] = selectList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("ParentCategoryId", "Title", "Content", "Slug")] CategoryProduct cate)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    // You can use a logger to log the errors or debug here
                    Console.WriteLine(error.ErrorMessage);
                    if (error.Exception != null)
                    {
                        Console.WriteLine(error.Exception.Message);
                    }
                }
                var categories = await (from c in _context.CategoryProducts select c).ToListAsync();

                categories.Insert(0, new CategoryProduct()
                {
                    Id = -1,
                    Title = "Không có danh mục"
                });

                var selectList = new SelectList(categories, "Id", "Title");
                ViewData["ParentCategoryId"] = selectList;

                return View(cate);
            }

            if (cate.ParentCategoryId == -1)
            {
                cate.ParentCategoryId = null;
            }
            await _context.AddAsync(cate);
            await _context.SaveChangesAsync();
            StatusMessage = "Success Created";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cate = _context.CategoryProducts.Where(c => c.Id == id).FirstOrDefault();
            if (cate == null)
            {
                return NotFound();
            }
            return View(cate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var cate = await _context.CategoryProducts.Include(c => c.ChildrenCategories)
                       .FirstOrDefaultAsync(c => c.Id == id);

            if (cate == null)
            {
                return NotFound();
            }

            if (cate.ChildrenCategories != null)
            {
                foreach (var child in cate.ChildrenCategories)
                {
                    child.ParentCategoryId = cate.ParentCategoryId;
                }
            }

            _context.Remove(cate);
            await _context.SaveChangesAsync();
            StatusMessage = "Delete Success";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cate = await _context.CategoryProducts.FirstOrDefaultAsync(c => c.Id == id);

            if (cate == null)
            {
                return NotFound();
            }

            return View(cate);
        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cate = await _context.CategoryProducts.FirstOrDefaultAsync(c => c.Id == id);

            if (cate == null)
            {
                return NotFound();
            }

            var categories = await (from c in _context.CategoryProducts select c)
                                   .Include(c => c.ChildrenCategories)
                                   .Include(c => c.ParentCategory).ToListAsync();
                                   

            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục",
                Slug = "#"
            });

            var selectList = new SelectList(categories, "Id", "Title", -1);

            ViewData["ParentCategoryId"] = selectList;

            return View(cate);
        }
        public bool checkValidParentCategory(CategoryProduct cate, int? targetId)
        {
            if (cate.Id == targetId)
            { 
                return false;
            }
            if (cate!=null)
            {
                var e = _context.Entry(cate);
                e.Collection(c => c.ChildrenCategories).Load();
            }
            foreach (var category in cate.ChildrenCategories.ToList())
            {
                bool result = checkValidParentCategory(category, targetId);
                if (result == false)
                {
                    return false;
                }
            }
            return true;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync([Bind("Title", "Content", "Slug", "ParentCategoryId")] CategoryProduct newCate, int id)
        {
            var cate = await (from p in _context.CategoryProducts
                              where p.Id == id
                              select p).FirstOrDefaultAsync();
            if (cate == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid && checkValidParentCategory(cate,newCate.ParentCategoryId))
            {
                if (newCate.ParentCategoryId == -1)
                {
                    newCate.ParentCategoryId = null;
                    _logger.LogInformation("ParentCategoryId = -1");
                }
                else
                {
                    cate.ParentCategoryId = newCate.ParentCategoryId;
                }
                cate.Slug = newCate.Slug;
                cate.Content = newCate.Content;
                cate.Title = newCate.Title;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            var categories = await (from c in _context.CategoryProducts select c)
                                   .Include(c => c.ChildrenCategories)
                                   .Include(c => c.ParentCategory).ToListAsync();

            categories.Insert(0, new CategoryProduct()
            {
                Id = -1,
                Title = "Không có danh mục",
                Slug = "#"
            });

            var selectList = new SelectList(categories, "Id", "Title", -1);
            StatusMessage = "Error, Cannot Edit";

            ViewData["ParentCategoryId"] = selectList;
            return View(newCate);
        }
    }
}
