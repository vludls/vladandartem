using System;
using System.Collections;
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
        public Product sProduct;
        private ProductContext myDb;
        private readonly IHostingEnvironment HostEnv;

        public HomeController(ProductContext context, IHostingEnvironment HostEnv)
        {
            myDb = context;
            
            this.HostEnv = HostEnv;
        }

        [HttpGet]
        public IActionResult Index(bool isSearch = false, string arg = "", int page = 0)
        {
            var ProductsArray = myDb.Products.ToList();
            
            List<Product> products = new List<Product>();
  
            int PagesCount = 1;

            arg = Request.Query.FirstOrDefault(p => p.Key == "arg").Value;

            PagesCount = (int)Math.Ceiling((decimal)((double)(myDb.Products.Count())/4));

            for(int i = page * 4, j = 1; i < ProductsArray.Count(); i++, j++)
            {
                if(j > 4) break;

                if(isSearch && !String.IsNullOrEmpty(arg))
                {
                    if(CompareTwoString(arg, ProductsArray[i].Name) < 50.0 &&
                    CompareTwoString(arg, ProductsArray[i].Manufacturer) < 50.0
                    )
                    {
                        j--;
                        continue;
                    }
                }

                products.Add(ProductsArray[i]);
            }

            ViewBag.Page = page;
            ViewBag.PagesCount = PagesCount;

            return View(products);
        }

        /*[HttpPost]
        public IActionResult Index(int id)
        {
            Product SomeProduct = myDb.Products.Find(id);

            myDb.Products.Remove(SomeProduct);

            myDb.SaveChanges();

            return View(myDb.Products.ToList());
        }*/

        [HttpGet]
        public IActionResult Add()
        {
            //ViewBag.Test = "Тест";
            return View();
        }
        
        [HttpGet]
        public IActionResult Cart()
        {
            //ViewBag.Test = "Тест";
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id, string name, int price, string imgpath, string manufacturer, string category)
        {
            ViewBag.Id = id;
            ViewBag.Name = name;
            ViewBag.Price = price;
            ViewBag.ImgPath = imgpath;
            ViewBag.Manufacturer = manufacturer;
            ViewBag.Category = category;

            return View();
        }
        [HttpPost]
        public IActionResult Edit(int id, string name, int price, IFormFile fileimg, string imgpath, string manufacturer, string category)
        {
            sProduct = myDb.Products.Find(id);

            if(fileimg != null)
            {
                string fileName;

                fileName = HostEnv.WebRootPath + "/images/Products/" + fileimg.FileName;

                fileimg.CopyTo(new FileStream(fileName, FileMode.Create));

                imgpath = "/images/Products/" + fileimg.FileName;
            }

            sProduct.Name = name;
            sProduct.Price = price;
            sProduct.ImgPath = imgpath;
            sProduct.Manufacturer = manufacturer;
            sProduct.Category = category;
        
            /*ViewBag.Id = id;
            ViewBag.Name = name;
            ViewBag.Price = price;
            ViewBag.ImgPath = imgpath;
            ViewBag.Manufacturer = manufacturer;
            ViewBag.Category = category;
            */
            ViewBag.SomeProduct = sProduct;
            //ViewBag.SomeProduct = SomeProduct;

            myDb.Products.Update(sProduct);

            myDb.SaveChanges();

            return View(sProduct);
        }
        [HttpPost]
        public IActionResult AddProduct(string name, int price, IFormFile fileimg, string manufacturer, string category)
        {
            string fileName;

            fileName = HostEnv.WebRootPath + "/images/Products/" + fileimg.FileName;

            fileimg.CopyTo(new FileStream(fileName, FileMode.Create));

            myDb.Products.AddRange(
                new Product{
                    Name = name,
                    Price = price,
                    ImgPath = "/images/Products/" + fileimg.FileName,
                    Manufacturer = manufacturer,
                    Category = category
                }
            );

            myDb.SaveChanges();
            //string someData = $"Название товара: {name} Цена товара: {price}";

            //return Content(Path.GetFileName(fileimg.FileName));
            //return Content(file.FileName);
            //ViewBag.Test = "Тест";

            return Redirect("~/Home/Index");
        }

        [HttpPost]
        public IActionResult RemoveProduct(int id)
        {
            Product SomeProduct = myDb.Products.Find(id);

            myDb.Products.Remove(SomeProduct);

            myDb.SaveChanges();

            return Redirect("~/Home/Index");
        }

        private double CompareTwoString(string strFirst, string strSecond)
        {
            string[] strFirstWordsArray = strFirst.Split(' ');
            string[] strSecondWordsArray = strSecond.Split(' ');

            int firstArrayWordsCount = strFirstWordsArray.Count();
            int secondsArrayWordsCount = strSecondWordsArray.Count();

            string[] MaxWordsArray; // = firstArrayWordsCount > secondsArrayWordsCount ? strFirstWordsArray : strSecondWordsArray;
            string[] MinWordsArray; // = firstArrayWordsCount < secondsArrayWordsCount ? strFirstWordsArray : strSecondWordsArray;
            
            if(firstArrayWordsCount > secondsArrayWordsCount)
            {
                MaxWordsArray = strFirstWordsArray;
                MinWordsArray = strSecondWordsArray;
            }
            else
            {
                MaxWordsArray = strSecondWordsArray;
                MinWordsArray = strFirstWordsArray;
            }

            int maxWordsCount = MaxWordsArray.Count();
            int minWordsCount = MinWordsArray.Count();

            int similar = 0;

            bool[] minWordsArrayChecked = new bool[minWordsCount];

            for(int i = 0; i < maxWordsCount; i++)
            {
                for(int j = 0; j < minWordsCount; j++)
                {
                    if(minWordsArrayChecked[j]) continue;

                    if(String.Equals(MaxWordsArray[i].ToLower(), MinWordsArray[j].ToLower()))
                    {
                        minWordsArrayChecked[j] = true;
                        similar++;
                        break;
                    }
                }
            }

            double percent = (double)(similar) / maxWordsCount * 100.0;

            return percent;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
