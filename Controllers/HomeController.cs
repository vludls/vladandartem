using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vladandartem.Models;

namespace vladandartem.Controllers
{
    public class HomeController : Controller
    {
        private ProductContext myDb;

        public HomeController(ProductContext context)
        {
            myDb = context;
        }

        public IActionResult Index()
        {
            return View(myDb.Products.ToList());
        }

        public IActionResult Privac()
        {
            return View();
        }

        [HttpGet]
        public IActionResult add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult add(string name, int price)
        {
            myDb.Products.AddRange(
                new Product{
                    Name = name,
                    Price = price
                }
            );

            myDb.SaveChanges();
            //string someData = $"Название товара: {name} Цена товара: {price}";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
