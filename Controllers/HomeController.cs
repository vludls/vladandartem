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
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.RegularExpressions;
using vladandartem.Models;
using vladandartem.ClassHelpers;
using vladandartem.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace vladandartem.Controllers
{
    public class HomeController : Controller
    {
        private ProductContext context;
        private readonly IHostingEnvironment HostEnv;

        private UserManager<User> userManager;

        public HomeController(ProductContext context, IHostingEnvironment HostEnv, UserManager<User> userManager)
        {
            context = context;

            this.HostEnv = HostEnv;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index(bool isSearch = false, string searchArgument = "", int page = 0)
        {
            // Если в поиск ничего не введено, то помечаем что пользователь не хочет искать
            if (String.IsNullOrEmpty(searchArgument)) isSearch = false;

            IEnumerable<Product> products;
            int productCounted = 0;

            // Если в поиск введено значение
            if (isSearch)
            {
                // Заносит продукт в массив, если строка в поиске совпадает на 50%
                // и более с названием товара
                products = context.Products.Where(n =>
                    CompareTwoString(searchArgument, n.Name) >= 50.0 ||
                    CompareTwoString(searchArgument, n.Manufacturer) >= 50.0
                );
            }
            // иначе
            else
            {
                // Берем элементов на 5 страниц
                products = context.Products.Take(4 * 5);
            }

            productCounted = products.Count();
            products = products.Skip(page * 4);
            products = products.Take(4);

            IndexViewModel ivm = new IndexViewModel
            {
                products = products,
                page = page,
                pagesCount = (int)Math.Ceiling((decimal)((double)(productCounted) / 4)),
                searchArgument = searchArgument
            };

            return View(ivm);
        }

        [HttpPost]
        public async Task<IActionResult> AddCookie(int id)
        {
            User user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return NotFound();
            }

            var buff = userManager.Users.Where(u => u.Id == user.Id).Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts)
                .FirstOrDefault();

            if (buff == null)
            {
                return NotFound();
            }

            if (buff.Cart.CartProducts.Find(p => p.ProductId == id) == null)
            {
                buff.Cart.CartProducts.Add(new CartProduct { ProductId = id, Count = 1 });
            }

            await userManager.UpdateAsync(buff);
            //user.Cart.CartProducts.Add(new CartProduct { Product = context.Products.Find(id), Count = 1});

            context.SaveChanges();
            //Cart cart = context.Users.FirstOrDefault(u => u.Cart.UserId == user.Id);

            /*Cart cart = new Cart(HttpContext.Session, "cart");

             cart.Decode();
             cart.Add(id);
             cart.Save();*/

            return RedirectToAction("Cart", "PersonalArea");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveProductCart(int id)
        {
            User user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return NotFound();
            }

            var buff = userManager.Users.Where(u => u.Id == user.Id).Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts).FirstOrDefault();

            buff.Cart.CartProducts.Remove(buff.Cart.CartProducts.Find(n => n.ProductId == id));

            await userManager.UpdateAsync(buff);
            /*Cart cart = new Cart(HttpContext.Session, "cart");

            cart.Decode();
            cart.Delete(id);
            cart.Save();*/

            return Redirect("~/Home/Cart");
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            EditViewModel evm = new EditViewModel
            {
                product = context.Products.Include(n => n.ProductDetailFieldDefinition)
                .ThenInclude(n => n.DetailFieldDefinition).First(n => n.Id == id),
                categories = context.Categories.ToList(),
                DetailFields = context.DetailFields.Include(n => n.DetailFieldDefinitions).ThenInclude(n => n.Definition).ToList()
            };

            return View(evm);
        }
        [HttpPost]
        public IActionResult EditAddDetailField(int ProductId, int DetailFieldId)
        {
           DetailFieldDefinition detailFieldDefinition = context.DetailFieldDefinitions.First(n => n.DetailFieldId == DetailFieldId);

            context.ProductDetailFieldDefinitions.Add(new ProductDetailFieldDefinition {
                ProductId = ProductId,
                DetailFieldDefinitionId = detailFieldDefinition.Id
            });

            return new EmptyResult();
        }
        [HttpPost]
        public IActionResult EditEditDetailField(int ProductId, int DetailFieldDefinitionId, int DetailFieldId, int DefinitionId)
        {
            DetailFieldDefinition detailFieldDefinition = context.
                DetailFieldDefinitions.First(n => n.DetailFieldId == DetailFieldId && n.DefinitionId == DefinitionId);


            context.ProductDetailFieldDefinitions.Add(new ProductDetailFieldDefinition
            {
                ProductId = ProductId,
                DetailFieldDefinitionId = detailFieldDefinition.Id
            });

            return new EmptyResult();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Edit(EditViewModel evm, IFormFile fileImg)
        {
            if (ModelState.IsValid)
            {
                if (fileImg != null)
                {
                    fileImg.CopyTo(new FileStream(
                        $"{HostEnv.WebRootPath}/images/Products/{fileImg.FileName}",
                        FileMode.Create)
                    );

                    evm.product.ImgPath = $"/images/Products/{fileImg.FileName}";
                }

                context.Products.Update(evm.product);

                context.SaveChanges();
            }

            evm.categories = context.Categories.ToList();

            return View(evm);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddViewModel { product = null, fileImg = null, categories = context.Categories.ToList() });
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Add(AddViewModel avm)
        {
            if (ModelState.IsValid)
            {
                string fileName;

                fileName = $"{HostEnv.WebRootPath}/images/Products/{avm.fileImg.FileName}";

                avm.fileImg.CopyTo(new FileStream(
                    $"{HostEnv.WebRootPath}/images/Products/{avm.fileImg.FileName}",
                    FileMode.Create
                    )
                );

                avm.product.ImgPath = $"/images/Products/{avm.fileImg.FileName}";

                context.Products.Add(avm.product);

                context.SaveChanges();

                return Redirect("~/Home/Index");
            }

            avm.categories = context.Categories.ToList();

            return View(avm);
        }

        [HttpPost]
        public IActionResult RemoveProduct(int id)
        {
            context.Products.Remove(context.Products.Find(id));

            context.SaveChanges();

            //return Redirect("~/Home/Index");

            return new EmptyResult();
        }

        private double CompareTwoString(string strFirst, string strSecond)
        {
            string[] strFirstWordsArray = strFirst.Split(' ');
            string[] strSecondWordsArray = strSecond.Split(' ');

            int firstArrayWordsCount = strFirstWordsArray.Count();
            int secondsArrayWordsCount = strSecondWordsArray.Count();

            string[] MaxWordsArray;
            string[] MinWordsArray;

            if (firstArrayWordsCount > secondsArrayWordsCount)
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

            for (int i = 0; i < maxWordsCount; i++)
            {
                for (int j = 0; j < minWordsCount; j++)
                {
                    if (minWordsArrayChecked[j]) continue;

                    if (String.Equals(MaxWordsArray[i].ToLower(), MinWordsArray[j].ToLower()))
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
        [HttpGet]
        public IActionResult CartChangeProductNum()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CartChangeProductNum(int id, int count)
        {
            User user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return NotFound();
            }

            var buff = userManager.Users.Where(u => u.Id == user.Id).Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts).ThenInclude(u => u.Product).FirstOrDefault();

            CartProduct cartProduct = buff.Cart.CartProducts.Find(n => n.ProductId == id);

            cartProduct.Count = count;

            await userManager.UpdateAsync(buff);
            /*Cart cart = new Cart(HttpContext.Session, "cart");

            cart.Decode();
            cart.Edit(id, count);
            cart.Save();
            */
            //Product product = context.Products.Find(id);

            return new JsonResult(Convert.ToString(cartProduct.Product.Count));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
