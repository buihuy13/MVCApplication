using Microsoft.AspNetCore.Mvc;
using MVCTest.Models.Product;

namespace App.Components
{
    [ViewComponent]
    public class CategoryProductSidebar : ViewComponent
    {
        public class CategoryProductSidebarData
        {
            public List<CategoryProduct> categories { get; set; }
            public int level { get; set; }
            public string categorySlug { get; set; }
        }
        public IViewComponentResult Invoke(CategoryProductSidebarData data)
        {
            return View(data);
        }
    }
}