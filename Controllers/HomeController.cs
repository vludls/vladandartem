using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Text.RegularExpressions;
using vladandartem.Models;
using Newtonsoft.Json;

namespace vladandartem.Controllers
{
    public class Errors
    {
        public List<string> ErrorMessages;

        public Errors()
        {
            ErrorMessages = new List<string>();
        }
    }
    public class Cart
    {
        private List<int> data;

        private ISession session;
        public Cart(ISession session)
        {
            data = new List<int>();
            this.session = session;
        }

        public void Add(int id)
        {
            if(!data.Contains(id))
                data.Add(id);
        }

        public List<int> Decode()
        {
            var SerializedString = session.GetString("cart");

            if(SerializedString != null)
                data = JsonConvert.DeserializeObject<List<int>>(SerializedString);

            return data;
        }

        public void Save()
        {
            session.SetString("cart", JsonConvert.SerializeObject(data));
        }

        public void Delete(int id)
        {
            if(data.Contains(id))
                data.Remove(id);
        }

        public int Count()
        {
            return data.Count();
        }
    }

    public class HomeController : Controller
    {
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
            if(String.IsNullOrEmpty(arg)) isSearch = false;

            var ProductsArray = myDb.Products.ToList();
            
            List<Product> products = new List<Product>();

            arg = Request.Query.FirstOrDefault(p => p.Key == "arg").Value;

            int ProductCounted = 0;

            for(int i = page * 4, j = 1; i < ProductsArray.Count(); i++, j++)
            {
                if(isSearch)
                {
                    if(CompareTwoString(arg, ProductsArray[i].Name) < 50.0 &&
                    CompareTwoString(arg, ProductsArray[i].Manufacturer) < 50.0
                    )
                    {
                        j--;
                        continue;
                    }
                    else
                    {
                        ProductCounted++;
                    }
                }

                if(j > 4) continue;

                products.Add(ProductsArray[i]);
            }

            ProductCounted = isSearch ? ProductCounted : ProductsArray.Count();

            ViewBag.Page = page;
            ViewBag.PagesCount = (int)Math.Ceiling((decimal)((double)(ProductCounted)/4));

            return View(products);
        }

        [HttpGet]
        public IActionResult Add()
        {
            //ViewBag.Test = "Тест";
            return View();
        }
        
        [HttpGet]
        public IActionResult Cart()
        {
            List<Product> Products = new List<Product>();

            Cart cart = new Cart(HttpContext.Session);

            foreach(var id in cart.Decode())
            {
                Product product = myDb.Products.Find(id);

                if(product != null)
                    Products.Add(product);
            }

            return View(Products);
        }

        [HttpPost]
        public IActionResult AddCookie(int id)
        {
            Cart cart = new Cart(HttpContext.Session);

            cart.Decode();
            cart.Add(id);
            cart.Save();

            return Redirect("~/Home/Cart");
        }
        [HttpPost]
        public IActionResult RemoveProductCart(int id)
        {
            Cart cart = new Cart(HttpContext.Session);

            cart.Decode();
            cart.Delete(id);
            cart.Save();

            return Redirect("~/Home/Cart");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(myDb.Products.Find(id));
        }
        [HttpPost]
        public IActionResult Edit(int id, string name, string price, IFormFile fileimg, string imgpath, string manufacturer, string category)
        {
            Product someProduct = myDb.Products.Find(id);

            Errors errors = CheckDataValidation(
                name,
                price,
                fileimg,
                manufacturer,
                category,
                imgpath
            );

            if(errors.ErrorMessages.Count() > 0)
            {
                ViewBag.Errors = errors;

                return View(someProduct);
            }

            if(fileimg != null)
            {
                string fileName;

                fileName = HostEnv.WebRootPath + "/images/Products/" + fileimg.FileName;

                fileimg.CopyTo(new FileStream(fileName, FileMode.Create));

                imgpath = "/images/Products/" + fileimg.FileName;
            }

            someProduct.Name = name;
            someProduct.Price = Convert.ToInt32(price);
            someProduct.ImgPath = imgpath;
            someProduct.Manufacturer = manufacturer;
            someProduct.Category = category;

            myDb.Products.Update(someProduct);

            myDb.SaveChanges();

            return View(someProduct);
        }
        [HttpPost]
        public IActionResult EditProduct(int id, string name, int price, IFormFile fileimg, string imgpath, string manufacturer, string category)
        {
            Product somePoduct = myDb.Products.Find(id);

            if(fileimg != null)
            {
                string fileName;

                fileName = HostEnv.WebRootPath + "/images/Products/" + fileimg.FileName;

                fileimg.CopyTo(new FileStream(fileName, FileMode.Create));

                imgpath = "/images/Products/" + fileimg.FileName;
            }

            somePoduct.Name = name;
            somePoduct.Price = price;
            somePoduct.ImgPath = imgpath;
            somePoduct.Manufacturer = manufacturer;
            somePoduct.Category = category;

            myDb.Products.Update(somePoduct);

            myDb.SaveChanges();

            return Redirect($"~/Home/Edit?id={id}");
        }
        [HttpPost]
        public IActionResult Add(string name, string price, IFormFile fileimg, string manufacturer, string category)
        {
            Errors errors = CheckDataValidation(
                name,
                price,
                fileimg,
                manufacturer,
                category
            );

            if(errors.ErrorMessages.Count() > 0)
            {
                ViewBag.Name = name;
                ViewBag.Price = price;
                ViewBag.Manufacturer = manufacturer;
                ViewBag.Category = category;

                return View(errors);
            }

            string fileName;

            fileName = HostEnv.WebRootPath + "/images/Products/" + fileimg.FileName;

            fileimg.CopyTo(new FileStream(fileName, FileMode.Create));

            myDb.Products.AddRange(
                new Product{
                    Name = name,
                    Price = Convert.ToInt32(price),
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

            return View();
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

        private Errors CheckDataValidation(string name, string price, IFormFile fileimg, string manufacturer, string category, string imgpath = "")
        {
            Errors errors = new Errors();

            if(fileimg == null && imgpath == "")
                errors.ErrorMessages.Add("Добавьте изображение товара!");

            if(String.IsNullOrEmpty(name)) 
                errors.ErrorMessages.Add("Заполните поле названия товара!");

            Regex regex = new Regex(@"\d+");

            if(String.IsNullOrEmpty(price))
                errors.ErrorMessages.Add("Заполните поле цены товара!");
            else
                if(!regex.IsMatch(price))
                    errors.ErrorMessages.Add("Цена может содержать только цифры!");

            if(String.IsNullOrEmpty(manufacturer))
                errors.ErrorMessages.Add("Заполните поле названия Производителя!");

            if(String.IsNullOrEmpty(category))
                errors.ErrorMessages.Add("Заполните поле названия Категории!");

            return errors;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
