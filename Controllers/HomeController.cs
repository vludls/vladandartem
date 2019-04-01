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
        public IActionResult PersArea()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Cart()
        {
            Cart cart = new Cart(HttpContext.Session, "cart");

            var products = from element in cart.Decode()
                        let buff = myDb.Products.Find(element.ProductId)
                        where buff != null
                        select new CartProduct {
                            ProductId = element.ProductId,
                            product = buff,
                            ProductCount = element.ProductCount
                        };

            return View(products);
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
            EditViewModel evm = new EditViewModel { 
                product = myDb.Products.Find(id),
                categories = myDb.Categories.ToList()
            };

            return View(evm);
        }

        [HttpPost]
        public IActionResult Edit(EditViewModel evm, IFormFile fileImg)
        {
            if(ModelState.IsValid)
            {
                if(fileImg != null)
                {
                    fileImg.CopyTo(new FileStream(
                        $"{HostEnv.WebRootPath}/images/Products/{fileImg.FileName}", 
                        FileMode.Create)
                    );

                    evm.product.ImgPath = $"/images/Products/{fileImg.FileName}";
                }

                myDb.Products.Update(evm.product);

                myDb.SaveChanges();
            }

            evm.categories = myDb.Categories.ToList(); 
            
            return View(evm);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddViewModel { product = null, fileImg = null, categories = myDb.Categories.ToList() });
        }
        [HttpPost]
        public IActionResult Add(AddViewModel avm)
        {
            if(ModelState.IsValid)
            {
                string fileName;

                fileName = $"{HostEnv.WebRootPath}/images/Products/{avm.fileImg.FileName}";

                avm.fileImg.CopyTo(new FileStream(
                    $"{HostEnv.WebRootPath}/images/Products/{avm.fileImg.FileName}", 
                    FileMode.Create
                    )
                );

                avm.product.ImgPath = $"/images/Products/{avm.fileImg.FileName}";

                myDb.Products.Add(avm.product);

                myDb.SaveChanges();

                return Redirect("~/Home/Index");
            }

            avm.categories = myDb.Categories.ToList();
            
            return View(avm);
        }

        [HttpPost]
        public IActionResult RemoveProduct(int id)
        {
            myDb.Products.Remove(myDb.Products.Find(id));

            myDb.SaveChanges();

            return Redirect("~/Home/Index");
        }

        private double CompareTwoString(string strFirst, string strSecond)
        {
            string[] strFirstWordsArray = strFirst.Split(' ');
            string[] strSecondWordsArray = strSecond.Split(' ');

            int firstArrayWordsCount = strFirstWordsArray.Count();
            int secondsArrayWordsCount = strSecondWordsArray.Count();

            string[] MaxWordsArray;
            string[] MinWordsArray;
            
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

            var errors = from element in cartData
                        let product = myDb.Products.Find(element.ProductId)
                        where element.ProductCount > product.Count
                        select $"Недопустимое количество товара: {product.Name}";

            if(errors.Any()) return View(errors);

            Cart cartPaid = new Cart(HttpContext.Session, "paid");

            cartPaid.Decode();
            
            if(cartData != null)
            {
                foreach(var element in cartData)
                {
                    cartPaid.Add(element.ProductId, element.ProductCount);

                    Product product = myDb.Products.Find(element.ProductId);

                    product.Count -= element.ProductCount;

                    myDb.Products.Update(product);

                    myDb.SaveChanges();
                }

                cartData.Clear();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
