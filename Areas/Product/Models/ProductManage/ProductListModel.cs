using MVCTest.Models;
using MVCTest.Models.Product;

namespace MVCApplication.Areas.Product.Models.ProductManage
{
    public class ProductListModel
    {
        public int totalPosts { get; set; }
        public int countPages { get; set; }
        public int ITEMS_PER_PAGE { get; set; } = 10;
        public int currentPage { get; set; }
        public List<ProductModel> ProductModels { get; set; }
    }
}
