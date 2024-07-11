using App.Data;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCTest.Models;

namespace MVCTest.Areas.Database.Controllers
{
    [Area("Database")]
    public class DbManageController : Controller
    {
        private readonly ILogger<DbManageController> logger;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbManageController(AppDbContext context,ILogger<DbManageController> _logger, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager) 
        {
            _context = context;
            logger = _logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteDb() 
        {
            return View();
        }
        [TempData]
        public string StatusMessage { get; set; }

        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var result = await _context.Database.EnsureDeletedAsync();
            if (result)
            {
                await _context.SaveChangesAsync();
                StatusMessage = "Delete Success";
            }
            else
            {
                logger.LogInformation("Cannot Delete");
                StatusMessage = "Error, Cannot Delete";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _context.Database.MigrateAsync();

            StatusMessage = "Update Success";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SeedDataAsync()
        {
            var roles = typeof(RoleName).GetFields().ToList();
            foreach (var role in roles) 
            {
                var roleName = (string)role.GetRawConstantValue();
                var rfound = await _roleManager.FindByNameAsync(roleName);
                if (rfound == null) 
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            //admin , admin123, admin@example.com
            var user = await _userManager.FindByEmailAsync("admin@example.com");
            if (user == null) 
            {
                user = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(user, "admin123");
                await _userManager.AddToRoleAsync(user, RoleName.Administrator);
                StatusMessage = "Seed Database Success";
            }
            SeedPostCategory();

            return RedirectToAction("Index");
        }

        private void SeedPostCategory()
        {
            var fakerCategory = new Faker<Category>();
            int cm = 1;
            fakerCategory.RuleFor(c => c.Title, fk => $"CM{cm++}" + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakerCategory.RuleFor(c => c.Content, fk => fk.Lorem.Sentences(1) + "[fakeData]");
            fakerCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());

            var cate1 = fakerCategory.Generate();
            var cate11 = fakerCategory.Generate();
            var cate12 = fakerCategory.Generate();
            var cate2 = fakerCategory.Generate();
            var cate21 = fakerCategory.Generate();
            var cate22 = fakerCategory.Generate();

            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;
            cate21.ParentCategory = cate2;
            cate22.ParentCategory = cate2;

            var rCateIndex = new Random();
            int bv = 1;
            var user = _userManager.GetUserAsync(this.User).Result;

            var fakerPost = new Faker<Post>();
            fakerPost.RuleFor(p => p.AuthorId, f => user.Id);
            fakerPost.RuleFor(p => p.Content, f => f.Lorem.Sentences(1)+"[fakeData]");
            fakerPost.RuleFor(p => p.DateCreated, f => f.Date.Between(new DateTime(2021,1,1), new DateTime(2021,7,1)));
            fakerPost.RuleFor(p => p.Description, f => f.Lorem.Sentences(2));
            fakerPost.RuleFor(p => p.Published, f => true);
            fakerPost.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            fakerPost.RuleFor(p => p.Title, f => $"Bai {bv++} " + f.Lorem.Sentence(3,4).Trim('.'));
            List<Post> posts = new List<Post>();    
            List<PostCategory> postCategories = new List<PostCategory>();

            var categories = new Category[] { cate11, cate12, cate2, cate1, cate21, cate22 };

            for (int i=0;i<40;i++)
            {
                var post = fakerPost.Generate();
                post.DateUpdated = post.DateCreated;
                posts.Add(post);
                postCategories.Add(new PostCategory()
                {
                    post = post,
                    cate = categories[rCateIndex.Next(5)]
                });
            }
            _context.AddRange(posts);
            _context.AddRange(postCategories);
            _context.AddRange(categories);

            _context.SaveChanges();
        }
    }
}

