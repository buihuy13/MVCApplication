using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using App.Areas.Identity.Models.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCApplication.Areas.Blog.Models.Posts;
using MVCApplication.Areas.Product.Models.ProductManage;
using MVCTest.Models;
using MVCTest.Models.Product;

namespace MVCTest.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class ProductManageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        [TempData]
        public string StatusMessage { get; set; }
        public ProductManageController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Blog/Posts
        public async Task<IActionResult> IndexAsync([FromQuery(Name = "p")] int currentPage)
        {
            var model = new ProductListModel();
            model.currentPage = currentPage;

            var qr = _context.Products.Include(p => p.Author).OrderBy(p => p.DateCreated);

            model.totalPosts = await qr.CountAsync();
            model.countPages = (int)Math.Ceiling((double)model.totalPosts / model.ITEMS_PER_PAGE);

            if (model.currentPage < 1)
                model.currentPage = 1;
            if (model.currentPage > model.countPages)
                model.currentPage = model.countPages;

            var qr1 = qr.Skip((model.currentPage - 1) * model.ITEMS_PER_PAGE)
                        .Take(model.ITEMS_PER_PAGE);

            model.ProductModels = await qr1.ToListAsync();
            return View(model);
        }

        // GET: Blog/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Blog/Posts/Create
        public IActionResult Create()
        {
            ViewData["CategoryIds"] = new MultiSelectList(_context.CategoryProducts.ToList(), "Id", "Title");
            return View();
        }

        // POST: Blog/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,CategoryIds")] CreateProductsModel product)
        {
            ViewData["CategoryIds"] = new MultiSelectList(_context.CategoryProducts.ToList(), "Id", "Title");
            if (await _context.Products.AnyAsync(p => p.Slug == product.Slug))
            {
                ModelState.AddModelError("Slug", "Use Another Url");
                return View(product);
            }
            if (product.CategoryIds == null)
            {
                StatusMessage = "Error, Must Choose CategoryId";
                return View(product);
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(this.User);
                product.DateCreated = DateTime.Now;
                product.DateUpdated = DateTime.Now;
                product.AuthorId = user.Id;
                product.Published = false;
                await _context.AddAsync(product);
                if (product.CategoryIds != null)
                {
                    foreach (var cateId in product.CategoryIds)
                    {
                        _context.Add(new ProductCategoryProduct()
                        {
                            CategoryProductId = cateId,
                            product = product,
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
            return View(product);
        }

        // GET: Blog/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.ProductCategoryProducts).FirstOrDefaultAsync(p => p.Id == id);

            ViewData["product"] = product;
            if (product == null)
            {
                return NotFound();
            }

            var newProduct = new CreateProductsModel()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                DateUpdated = product.DateUpdated,
                Title = product.Title,
                Description = product.Description,
                Content = product.Content,
                Slug = product.Slug,
                Published = product.Published,
                AuthorId = product.AuthorId,
                CategoryIds = product.ProductCategoryProducts.Select(p => p.CategoryProductId).ToArray()
            };

            ViewData["CategoryIds"] = new MultiSelectList(_context.CategoryProducts.ToList(), "Id", "Title");
            return View(newProduct);
        }

        // POST: Blog/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Description,Slug,Content,CategoryIds")] CreateProductsModel model)
        {
            var product = await _context.Products.Include(p => p.ProductCategoryProducts).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    product.Slug = model.Slug;
                    product.Title = model.Title;
                    product.Description = model.Description;
                    product.Content = model.Content;
                    product.DateUpdated = DateTime.Now;

                    if (model.CategoryIds == null)
                    {
                        StatusMessage = "Error, Must Choose CategoryId";
                        return View(model);
                    }

                    var oldCate = product.ProductCategoryProducts.Select(p => p.CategoryProductId).ToArray();
                    var newCate = model.CategoryIds;
                    var addnewCate = newCate.Where(c => !oldCate.Contains(c));
                    var deleteCate = from postcate in product.ProductCategoryProducts
                                     where !newCate.Contains(postcate.CategoryProductId)
                                     select postcate;

                    _context.ProductCategoryProducts.RemoveRange(deleteCate);

                    foreach (var categoryId in addnewCate)
                    {
                        _context.Add(new ProductCategoryProduct()
                        {
                            CategoryProductId = categoryId,
                            ProductId = id
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(model.Id))
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
            ViewData["product"] = product;
            ViewData["AuthorId"] = new SelectList(_context.Categories.ToList(), "Id", "Title");
            return RedirectToAction("Index");
        }

        // GET: Blog/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Blog/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public class UploadOneFile
        {
            [Required]
            [DataType(DataType.Upload)]
            [FileExtensions(Extensions = "png,jpg,jpeg,gif")]
            [DisplayName("Choose File To Upload")]
            public IFormFile FileUpload { get; set; }
        }

        [HttpGet]
        public IActionResult UploadPhoto(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _context.Products.Include(p => p.Photos).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound("No Product With Such Id");
            }
            ViewData["Product"] = product;
            return View(new UploadOneFile());
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhotoAsync(int id, [Bind] UploadOneFile? f)
        {
            var product = _context.Products.Include(p => p.Photos).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(f.FileUpload.FileName);

                var file = Path.Combine("Uploads", "Products", file1);

                using var filestream = new FileStream(file, FileMode.Create);
                await f.FileUpload.CopyToAsync(filestream);

                await _context.AddAsync(new ProductPhoto()
                {
                    ProductId = id,
                    FileName = file1
                });

                await _context.SaveChangesAsync();
            }

            ViewData["Product"] = product;
            return View(new UploadOneFile());
        }

        [HttpPost]
        public async Task<IActionResult> ListPhotos(int id)
        {
            var product = await _context.Products.Where(p => p.Id == id)
                                           .Include(p => p.Photos)
                                           .FirstOrDefaultAsync();

            if (product == null)
            {
                return Json(
                    new
                    {
                        success = 0,
                        message = "Product Not Found"
                    });
            }

            var listphotos = product.Photos.Select(photo => new
            {
                id = photo.Id,
                path = "/contents/Products/" + photo.FileName
            });

            return Json(new
            {
                success = 1,
                photos = listphotos
            });
        }
        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var photo = await _context.productPhotos.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (photo != null)
            {
                _context.Remove(photo);
                await _context.SaveChangesAsync();

                var fileName = "Uploads/Products/" + photo.FileName;
                System.IO.File.Delete(fileName);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhotoApi(int id, [Bind("FileUpload")] UploadOneFile? f)
        {
            var product = _context.Products.Include(p => p.Photos).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(f.FileUpload.FileName);

                var file = Path.Combine("Uploads", "Products", file1);

                using var filestream = new FileStream(file, FileMode.Create);
                await f.FileUpload.CopyToAsync(filestream);

                await _context.AddAsync(new ProductPhoto()
                {
                    ProductId = id,
                    FileName = file1
                });

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
