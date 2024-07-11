using Microsoft.AspNetCore.Mvc;

namespace MVCTest.Controllers
{
    public class FirstController : Controller
    {
        public FirstController() {

        }
        public string Index()
        {
            // this.HttpContext
            // this.Request
            // this.Response
            // this.RouteData
            
            // this.User
            // this.ModelState
            // this.ViewData
            // this.Url
            // this.TempData
            // ...
            return "This is first Controller";
        }
        
        public IActionResult BirdsImage()
        {
            var builder = WebApplication.CreateBuilder();
            string filepath = Path.Combine(builder.Environment.ContentRootPath, "Files", "Birds.jpg");

            var bytes = System.IO.File.ReadAllBytes(filepath);

            return File(bytes, "image/jpg");
        }

        public IActionResult HelloView(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = "Customer";
            }

            return View((object)userName);
        }

    }
}
