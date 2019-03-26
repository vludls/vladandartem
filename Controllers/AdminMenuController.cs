using Microsoft.AspNetCore.Mvc;
using vladandartem.Models;

namespace vladandartem.Controllers
{
    public class AdminMenuController : Controller
    {
        private ProductContext myDb;
        //private readonly IHostingEnvironment HostEnv;

        public AdminMenuController(ProductContext context)
        {
            myDb = context;
        }
        public IActionResult Main()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCategory(string CategoryName)
        {
            myDb.Categories.Add(new Category{ Name = CategoryName });

            myDb.SaveChanges();
            
            return Redirect("~/AdminMenu/Main");
        }
    }
}