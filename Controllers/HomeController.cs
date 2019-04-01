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
using vladandartem.ClassHelpers;
using vladandartem.ViewModels;

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
        public IActionResult Index(bool isSearch = false, string searchArgument = "", int page = 0)
        {
            // Если в поиск ничего не введено, то помечаем что пользователь не хочет искать
            if(String.IsNullOrEmpty(searchArgument)) isSearch = false;

            IEnumerable<Product> products;
            int productCounted = 0;

            // Если в поиск введено значение
            if(isSearch)
            {
                // Заносит продукт в массив, если строка в поиске совпадает на 50%
                // и более с названием товара
                products = myDb.Products.Where(n => 
                    CompareTwoString(searchArgument, n.Name) >= 50.0 ||
                    CompareTwoString(searchArgument, n.Manufacturer) >= 50.0
                );
            }
            // иначе
            else
            {
                // Берем элементов на 5 страниц
                products = myDb.Products.Take(4*5);
            }

            productCounted = products.Count();
            products = products.Skip(page * 4);
            products = products.Take(4);

            IndexViewModel ivm = new IndexViewModel {
                products = products,
                page = page,
                pagesCount = (int)Math.Ceiling((decimal)((double)(productCounted)/4)),
                searchArgument = searchArgument
            };

            return View(ivm);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.CategoriesList = myDb.Categories.ToList();
            //ViewBag.Test = "Тест";
            return View();
        }

        [HttpGet]
        public IActionResult PersArea()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Cart()
        {
            List<CartProduct> Products = new List<CartProduct>();

            Cart cart = new Cart(HttpContext.Session, "cart");

            foreach(var sessionProduct in cart.Decode())
            {
                Product productBuff = myDb.Products.Find(sessionProduct.ProductId);
                CartProduct cartProduct = sessionProduct;

                if(productBuff != null)
                {
                    cartProduct.product = productBuff;
                    Products.Add(cartProduct);
                }
            }

            return View(Products);
        }

        [HttpPost]
        public IActionResult AddCookie(int id)
        {
            Cart cart = new Cart(HttpContext.Session, "cart");

            cart.Decode();
            cart.Add(id);
            cart.Save();

            return Redirect("~/Home/Cart");
        }
        [HttpPost]
        public IActionResult RemoveProductCart(int id)
        {
            Cart cart = new Cart(HttpContext.Session, "cart");

            cart.Decode();
            cart.Delete(id);
            cart.Save();

            return Redirect("~/Home/Cart");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.CategoriesList = myDb.Categories.ToList();

            return View(myDb.Products.Find(id));
        }
        /*[HttpPost]
        public IActionResult Edit(int id, string name, string price, IFormFile fileimg, string imgpath, string manufacturer, int categoryId, int count)
        {
            Product someProduct = myDb.Products.Find(id);

            Errors errors = CheckDataValidation(
                name,
                price,
                fileimg,
                manufacturer,
                categoryId,
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
            someProduct.CategoryId = categoryId;
            someProduct.Count = count;

            myDb.Products.Update(someProduct);

            myDb.SaveChanges();

            ViewBag.CategoriesList = myDb.Categories.ToList();

            return View(someProduct);
        }*/
        [HttpPost]
        public IActionResult Edit(Product product, string imgpath)
        {
            if(!ModelState.IsValid)
            {
                return Content("Работает");
            }
            Product someProduct = myDb.Products.Find(product.Id);

            /*Errors errors = CheckDataValidation(
                name,
                price,
                fileimg,
                manufacturer,
                categoryId,
                imgpath
            );

            if(errors.ErrorMessages.Count() > 0)
            {
                ViewBag.Errors = errors;

                return View(someProduct);
            }
            */
            /*if(fileimg != null)
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
            someProduct.CategoryId = categoryId;
            someProduct.Count = count;

            myDb.Products.Update(someProduct);

            myDb.SaveChanges();

            ViewBag.CategoriesList = myDb.Categories.ToList();
            */
            return View(product);
        }
        [HttpPost]
        public IActionResult Add(string name, string price, IFormFile fileimg, string manufacturer, int categoryId, int count)
        {
            Errors errors = CheckDataValidation(
                name,
                price,
                fileimg,
                manufacturer,
                categoryId
            );

            if(errors.ErrorMessages.Count() > 0)
            {
                ViewBag.Name = name;
                ViewBag.Price = price;
                ViewBag.Manufacturer = manufacturer;
                ViewBag.CategoryId = categoryId;

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
                    CategoryId = categoryId,
                    Count = count
                }
            );

            myDb.SaveChanges();
            //string someData = $"Название товара: {name} Цена товара: {price}";

            //return Content(Path.GetFileName(fileimg.FileName));
            //return Content(file.FileName);
            //ViewBag.Test = "Тест";

            
            ViewBag.CategoriesList = myDb.Categories.ToList();
            
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

        [HttpPost]
        public IActionResult CartBuy()
        {
            Cart cart = new Cart(HttpContext.Session, "cart");

            List<CartProduct> cartData = cart.Decode();

            Errors error = new Errors();

            foreach(var element in cartData)
            {
                Product product = myDb.Products.Find(element.ProductId);

                if(element.ProductCount > product.Count)
                {
                    error.ErrorMessages.Add($"Недопустимое количество товара: {product.Name}");
                }
                // Тут дальше делать
            }

            if(error.ErrorMessages.Count() > 0)
            {
                return View(error);
            }

            Cart cartPaid = new Cart(HttpContext.Session, "paid");

            cartPaid.Decode();


            if(cart.Decode() != null)
            {
                foreach(var element in cartData)
                {
                    cartPaid.Add(element.ProductId, element.ProductCount);

                    Product product = myDb.Products.Find(element.ProductId);

                    product.Count -= element.ProductCount;

                    myDb.Products.Update(product);

                    myDb.SaveChanges();

                    cart.Delete(element.ProductId);
                }
            }

            cart.Save();
            cartPaid.Save();

            return Redirect("~/PersonalArea/Main");
        }
        [HttpGet]
        public IActionResult CartChangeProductNum()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CartChangeProductNum(int id, int count)
        {
            Cart cart = new Cart(HttpContext.Session, "cart");

            cart.Decode();
            cart.Edit(id, count);
            cart.Save();

            Product product = myDb.Products.Find(id);

            return new JsonResult(Convert.ToString(product.Count));
        }
        private Errors CheckDataValidation(string name, string price, IFormFile fileimg, string manufacturer, int categoryId, string imgpath = "")
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

            /*if(String.IsNullOrEmpty(categoryId))
                errors.ErrorMessages.Add("Заполните поле названия Категории!");*/

            return errors;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
