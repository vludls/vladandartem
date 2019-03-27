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
    public struct CartProduct
    {
        public int ProductId;
        public int ProductCount;
    }
    public class Cart
    {
        List<CartProduct> cartProduct = new List<CartProduct>();

        private readonly string FieldName;

        private ISession session;

        public Cart(ISession session, string FieldName)
        {
            this.session = session;
            this.FieldName = FieldName;
        }

        public void Add(int id, int count = 1)
        {
            if(!cartProduct.Any(product => product.ProductId == id))
            {
                cartProduct.Add(new CartProduct{ ProductId = id, ProductCount = count });
            }
        }
        public List<CartProduct> Decode()
        {
            var SerializedString = session.GetString(FieldName);

            if(SerializedString != null)
                cartProduct = JsonConvert.DeserializeObject<List<CartProduct>>(SerializedString);

            return cartProduct;
        }

        public void Save()
        {
            session.SetString(FieldName, JsonConvert.SerializeObject(cartProduct));
        }

        public void Delete(int id)
        {   
            if(cartProduct.Any(product => product.ProductId == id))
            {
                CartProduct productBuff = cartProduct.Find(product => product.ProductId == id);

                cartProduct.Remove(productBuff);
            }
        }

        public void Edit(int id, int count)
        {
            var product = cartProduct.FirstOrDefault(productBuff => productBuff.ProductId == id);

            product.ProductCount = count;
            //cartProduct.Ele.ProductCount = count;
        }

        public int Count()
        {
            return cartProduct.Count();
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
            ViewBag.Argument = arg;

            return View(products);
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
            //ViewBag.Test = "Тест";
            return View();
        }
        
        [HttpGet]
        public IActionResult Cart()
        {
            List<Product> Products = new List<Product>();

            Cart cart = new Cart(HttpContext.Session, "cart");

            foreach(var product in cart.Decode())
            {
                Product productBuff = myDb.Products.Find(product.ProductId);

                if(productBuff != null)
                    Products.Add(productBuff);
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
        [HttpPost]
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
        }
        /*[HttpPost]
        public IActionResult EditProduct(int id, string name, int price, IFormFile fileimg, string imgpath, string manufacturer, int categoryId)
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
            somePoduct.CategoryId = categoryId;

            myDb.Products.Update(somePoduct);

            myDb.SaveChanges();

            return Redirect($"~/Home/Edit?id={id}");
        }*/
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
            Cart cartPaid = new Cart(HttpContext.Session, "paid");

            List<CartProduct> cartData = cart.Decode(); 
            cartPaid.Decode();


            if(cart.Decode() != null)
            {
                foreach(var product in cartData)
                {
                    cartPaid.Add(product.ProductId, product.ProductCount);
                    cart.Delete(product.ProductId);
                }
            }

            cart.Save();
            cartPaid.Save();

            return Redirect("~/PersonalArea/Main");
        }
        [HttpPost]
        public IActionResult CartChangeProductNum(int id, int count)
        {
            Cart cart = new Cart(HttpContext.Session, "cart");

            cart.Decode();
            cart.Edit(id, count);
            cart.Save();

            return new EmptyResult();
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
