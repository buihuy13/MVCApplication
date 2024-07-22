using MVCTest.Models.Product;
using Newtonsoft.Json;
using System.Globalization;

namespace MVCApplication.Areas.Product.Models.Cart
{
    public class CartItem
    {
        public int quantity { set; get; }
        public ProductModel product { set; get; }
      
    }
}
