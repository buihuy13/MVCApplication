using MVCApplication.Areas.Product.Models.Cart;
using Newtonsoft.Json;

namespace MVCApplication.Areas.Product.Service
{
    public class CartService
    {
        //Key lưu chuỗi json của Cart
        public const string CARTKEY = "cart";

        private readonly HttpContext httpContext;

        private readonly IHttpContextAccessor _context;

        public CartService(IHttpContextAccessor context)
        {
            httpContext = context.HttpContext;
            _context = context;
        }

        // Lấy cart từ Session (danh sách CartItem)
        public List<CartItem> GetCartItems()
        {
            var session = httpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }

        // Xóa cart khỏi session
        public void ClearCart()
        {
            var session = httpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        public void SaveCartSession(List<CartItem> ls)
        {
            var session = httpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

    }
}
