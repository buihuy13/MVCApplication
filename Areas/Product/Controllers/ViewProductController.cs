using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MVCApplication.Areas.Product.Models.Cart;
using MVCApplication.Areas.Product.Service;
using MVCTest.Models;
using MVCTest.Models.Product;

namespace MVCTest.Areas.Product.Controllers
{
    [Area("Product")]
    public class ViewProductController : Controller
    {
        private readonly ILogger<ViewProductController> logger;
        private readonly AppDbContext _context;
        private readonly CartService cartService;
        public ViewProductController(ILogger<ViewProductController> _logger, AppDbContext context, CartService cartService)
        {
            logger = _logger;
            _context = context;
            this.cartService = cartService;
        }

        [Route("/product/{categoryslug?}")]
        public async Task<IActionResult> Index(string? categoryslug, int? page)
        {
            var categories = GetCategories();
            ViewBag.Categories = categories;
            ViewBag.categoryslug = categoryslug;

            CategoryProduct category = null;

            if (!string.IsNullOrEmpty(categoryslug))
            {
                category = await _context.CategoryProducts.Where(c => c.Slug == categoryslug)
                                                  .Include(c => c.ChildrenCategories)
                                                  .FirstOrDefaultAsync();

                if (category == null)
                {
                    return NotFound();
                }
            }

            var products = _context.Products.
                        Include(p => p.Author).
                        Include(p => p.Photos).
                        Include(p => p.ProductCategoryProducts).
                        ThenInclude(p => p.categoryproduct).
                        AsQueryable();

            products.OrderBy(p => p.DateUpdated);

            var parentIds = new List<int>();

            if (category != null)
            {
                var ids = new List<int>();
                category.CategoryChildrenIds(category.ChildrenCategories, ids);
                ids.Add(category.Id);

                products = products.Where(p => p.ProductCategoryProducts.Where(pc => ids.Contains(pc.CategoryProductId)).Any());

                category.ParentCategoryIds(category, parentIds);
            }

            List<CategoryProduct> parentCates = new List<CategoryProduct>();
            if (parentIds.Count > 0)
            {
               parentIds.RemoveAt(0);
                for (int i = parentIds.Count() - 1;i>=0;i--)
                {
                    parentCates.Add(_context.CategoryProducts.Where(c => c.Id == parentIds[i]).FirstOrDefault());
                }
            }

            ViewBag.parentCates = parentCates;
            ViewBag.Category = category;
            return View(products.ToList());
        }

        [Route("/product/{productslug}.html")]
        public IActionResult Detail(string productslug)
        {
            return View();
        }
        private List<CategoryProduct> GetCategories()
        {
            var categories = _context.CategoryProducts.Include(c => c.ChildrenCategories)
                                                .AsEnumerable()
                                                .Where(c => c.ParentCategory == null)
                                                .ToList();
            return categories;
        }

        /// Thêm sản phẩm vào cart
        [Route("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart([FromRoute] int productid)
        {

            var product = _context.Products
                .Where(p => p.Id == productid)
                .FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            var cart = cartService.GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { quantity = 1, product = product });
            }

            // Lưu cart vào Session
            cartService.SaveCartSession(cart);
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(Cart));
        }

        // Hiện thị giỏ hàng
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return View(cartService.GetCartItems());
        }

        /// xóa item trong cart
        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = cartService.GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == productid);
            if (cartitem != null)
            {
                cart.Remove(cartitem);
            }

            cartService.SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }

        /// Cập nhật
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = cartService.GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == productid);
            if (cartitem != null)
            {
                cartitem.quantity = quantity;
            }
            cartService.SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }
    }
}
