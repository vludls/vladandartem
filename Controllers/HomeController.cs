using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using vladandartem.Models;

namespace vladandartem.Controllers
{
    public class HomeController : Controller
    {
        private ProductContext myDb;
        private readonly IHostingEnvironment HostEnv;

        public HomeController(ProductContext context, IHostingEnvironment HostEnv)
        {
            myDb = context;
            
            this.HostEnv = HostEnv;
        }

        public IActionResult Index()
        {
            return View(myDb.Products.ToList());
        }

        [HttpPost]
        public IActionResult Index(int id)
        {
            Product SomeProduct = myDb.Products.Find(id);

            myDb.Products.Remove(SomeProduct);

            myDb.SaveChanges();

            return View(myDb.Products.ToList());
        }

        public IActionResult Privac()
        {
            return View();
        }

        [HttpGet]
        public IActionResult add()
        {
            //ViewBag.Test = "Тест";
            return View();
        }
        
        [HttpGet]
        public IActionResult redact(int id, string name, int price, string imgpath)
        {
            ViewBag.Id = id;
            ViewBag.Name = name;
            ViewBag.Price = price;
            ViewBag.ImgPath = imgpath;

            return View();
        }
        [HttpPost]
        public IActionResult redact(int id, string name, int price, IFormFile fileimg, string imgpath)
        {
            Product SomeProduct = myDb.Products.Find(id);

            if(fileimg != null)
            {
                string fileName;

                fileName = HostEnv.WebRootPath + "/images/Products/" + fileimg.FileName;

                fileimg.CopyTo(new FileStream(fileName, FileMode.Create));

                imgpath = "/images/Products/" + fileimg.FileName;
            }

            SomeProduct.Name = name;
            SomeProduct.Price = price;
            SomeProduct.ImgPath = imgpath;

            myDb.Products.Update(SomeProduct);

            myDb.SaveChanges();
            

            ViewBag.Id = id;
            ViewBag.Name = name;
            ViewBag.Price = price;
            ViewBag.ImgPath = imgpath;
            
            return View();
        }

        [HttpPost]
        public IActionResult add(string name, int price, IFormFile fileimg)
        {
            string fileName;

            fileName = HostEnv.WebRootPath + "/images/Products/" + fileimg.FileName;

            fileimg.CopyTo(new FileStream(fileName, FileMode.Create));

            myDb.Products.AddRange(
                new Product{
                    Name = name,
                    Price = price,
                    ImgPath = "/images/Products/" + fileimg.FileName
                }
            );

            myDb.SaveChanges();
            //string someData = $"Название товара: {name} Цена товара: {price}";

            //return Content(Path.GetFileName(fileimg.FileName));
            //return Content(file.FileName);
            //ViewBag.Test = "Тест";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
