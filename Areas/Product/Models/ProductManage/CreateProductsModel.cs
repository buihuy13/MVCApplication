using MVCTest.Models;
using MVCTest.Models.Product;

namespace MVCApplication.Areas.Product.Models.ProductManage
{
    public class CreateProductsModel : ProductModel
    {
        public int[] CategoryIds { get; set; }
    }
}
