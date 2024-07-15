using Microsoft.AspNetCore.Mvc;
using MVCTest.Models;

namespace App.Components
{
    [ViewComponent]
    public class CategorySidebar : ViewComponent
    {
        public class CategorySidebarData
        {
            public List<Category> categories { get; set; }
            public int level { get; set; }
            public string categorySlug { get; set; }
        }
        public IViewComponentResult Invoke(CategorySidebarData data)
        {
            return View(data);
        }
    }
}