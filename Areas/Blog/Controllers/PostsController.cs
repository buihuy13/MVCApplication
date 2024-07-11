using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Areas.Identity.Models.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCApplication.Areas.Blog.Models.Posts;
using MVCTest.Models;

namespace MVCApplication.Areas.Blog
{
    [Area("Blog")]
    [Authorize]
    public class PostsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        [TempData]
        public string StatusMessage { get; set; }

        public PostsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Blog/Posts
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage)
        {
            var model = new PostsListModel();
            model.currentPage = currentPage;

            var qr = _context.Posts.Include(p => p.Author).OrderBy(p => p.DateCreated);

            model.totalPosts = await qr.CountAsync();
            model.countPages = (int)Math.Ceiling((double)model.totalPosts / model.ITEMS_PER_PAGE);

            if (model.currentPage < 1)
                model.currentPage = 1;
            if (model.currentPage > model.countPages)
                model.currentPage = model.countPages;

            var qr1 = qr.Skip((model.currentPage - 1) * model.ITEMS_PER_PAGE)
                        .Take(model.ITEMS_PER_PAGE);

            model.posts = await qr1.ToListAsync();
            return View(model);
        }

        // GET: Blog/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Blog/Posts/Create
        public IActionResult Create()
        {
            ViewData["CategoryIds"] = new MultiSelectList(_context.Categories.ToList(), "Id", "Title");
            return View();
        }

        // POST: Blog/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,CategoryIds")]CreatePostsModel post)
        {
            ViewData["CategoryIds"] = new MultiSelectList(_context.Categories.ToList(), "Id", "Title");
            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug))
            {
                ModelState.AddModelError("Slug", "Use Another Url");
                return View(post);
            }
            if (post.CategoryIds == null)
            {
                StatusMessage = "Error, Must Choose CategoryId";
                return View(post);
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(this.User);
                post.DateCreated = DateTime.Now;
                post.DateUpdated = DateTime.Now;
                post.AuthorId = user.Id;
                post.Published = false;
                await _context.AddAsync(post);
                if (post.CategoryIds != null)
                {
                    foreach (var cateId in post.CategoryIds)
                    {
                        _context.Add(new PostCategory()
                        {
                            CategoryId = cateId,
                            post = post,
                        });
                    }
                }
                await _context.SaveChangesAsync();
                StatusMessage = "Success Created";
                return RedirectToAction(nameof(Index));
            }
            else
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
            }
            StatusMessage = "Error, Cannot Create";
            return View(post);
        }

        // GET: Blog/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var newPost = new CreatePostsModel()
            {
                Id = post.Id,
                DateCreated = post.DateCreated,
                DateUpdated = post.DateUpdated,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Slug = post.Slug,
                Published = post.Published,
                AuthorId = post.AuthorId,
                CategoryIds = post.PostCategories.Select(p => p.CategoryId).ToArray()
            };

            ViewData["CategoryIds"] = new MultiSelectList(_context.Categories.ToList(), "Id", "Title");
            return View(newPost);
        }

        // POST: Blog/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Description,Slug,Content,CategoryIds")]CreatePostsModel model)
        {
            var post = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    post.Slug = model.Slug;
                    post.Title = model.Title;
                    post.Description = model.Description;
                    post.Content = model.Content;
                    post.DateUpdated = DateTime.Now;

                    if (model.CategoryIds == null)
                    {
                        StatusMessage = "Error, Must Choose CategoryId";
                        return View(model);
                    }

                    var oldCate = post.PostCategories.Select(p => p.CategoryId).ToArray();
                    var newCate = model.CategoryIds;
                    var addnewCate = newCate.Where(c => !oldCate.Contains(c));
                    var deleteCate = from postcate in post.PostCategories
                                     where !newCate.Contains(postcate.CategoryId)
                                     select postcate;

                    _context.PostCategories.RemoveRange(deleteCate);

                    foreach (var categoryId in addnewCate)
                    {
                        _context.Add(new PostCategory()
                        {
                            CategoryId = categoryId,
                            PostId = id
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Categories.ToList(), "Id", "Title");
            return View(model);
        }

        // GET: Blog/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Blog/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
