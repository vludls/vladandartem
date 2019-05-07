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
using vladandartem.Data.Models;
using vladandartem.Models.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using vladandartem.Models.Request.Home;

namespace vladandartem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductContext _context;
        private readonly UserManager<User> _userManager;

        private readonly IHostingEnvironment _hostEnv;

        public HomeController(ProductContext context, UserManager<User> userManager, IHostingEnvironment hostEnv)
        {
            _context = context;
            _userManager = userManager;
            _hostEnv = hostEnv;
        }

        [HttpGet]
        public ViewResult Index(int page = 0, string searchArgument = "")
        {
            IEnumerable<Product> products;

            // Если в поиск введено значение
            if (searchArgument != "" && searchArgument != null)
            {
                products = _context.Products
                    .Where(n =>
                    IsSimilar(searchArgument, n.Name) ||
                    IsSimilar(searchArgument, n.Manufacturer)
                );
            }
            // иначе
            else
            {
                // Берем элементов на 5 страниц
                products = _context.Products.Take(4 * 5);
            }

            int productCounted = products.Count();
            products = products.Skip(page * 4).Take(4);

            IndexViewModel ivm = new IndexViewModel
            {
                Products = products,
                Page = page,
                PagesCount = (int)Math.Ceiling((decimal)((double)(productCounted) / 4)),
                SearchArgument = searchArgument
            };

            return View(ivm);
        }

        [HttpPost]
        [Authorize]
        public async Task<RedirectToActionResult> AddCookie(int id)
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            user = _userManager.Users.Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts)
                .FirstOrDefault(u => u.Id == user.Id);

            if (!user.Cart.CartProducts.Any(p => p.ProductId == id))
                user.Cart.CartProducts.Add(new CartProduct { ProductId = id, Count = 1 });

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Cart", "PersonalArea");
        }

        [HttpPost]
        [Authorize]
        public async Task<ContentResult> RemoveProductCart(int cartProductId)
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            user = _userManager.Users.Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts)
                .FirstOrDefault(u => u.Id == user.Id);

            CartProduct cartProduct = user.Cart.CartProducts.FirstOrDefault(n => n.Id == cartProductId);

            if (cartProduct == null)
                return Content(JsonConvert.SerializeObject(new { CartProductId = 0 }));

            user.Cart.CartProducts.Remove(cartProduct);

            await _userManager.UpdateAsync(user);

            return Content(JsonConvert.SerializeObject(new { CartProductId = cartProductId }));
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ViewResult Edit(int id)
        {
            EditViewModel viewModel = new EditViewModel
            {
                Product = _context.Products.Include(n => n.ProductDetailFields).ThenInclude(n => n.DetailField)
                .ThenInclude(n => n.Definitions).FirstOrDefault(n => n.Id == id),
                Categories = _context.Categories.ToList(),
                DetailFields = _context.DetailFields.Include(n => n.Definitions).ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult EditAddDetailField([Required]int ProductId, [Required]int DetailFieldId)
        {
            if (ModelState.IsValid)
            {
                if (_context.Products.FirstOrDefault(n => n.Id == ProductId) != null &&
                _context.DetailFields.FirstOrDefault(n => n.Id == DetailFieldId) != null)
                {
                    _context.ProductDetailFields.Add(new ProductDetailField { ProductId = ProductId, DetailFieldId = DetailFieldId });
                    _context.SaveChanges();
                }
            }

            return new EmptyResult();
        }
        [HttpPost]
        public EmptyResult EditDeleteDetailField([Required]int ProductDetailFieldId)
        {
            if (ModelState.IsValid)
            {
                ProductDetailField productDetailField = _context.ProductDetailFields.FirstOrDefault(n => n.Id == ProductDetailFieldId);

                if (productDetailField != null)
                {
                    _context.ProductDetailFields.Remove(productDetailField);
                    _context.SaveChanges();
                }
            }

            return new EmptyResult();
        }

        /*[HttpPost]
        public IActionResult EditEditDetailField(int ProductDetailFieldId, int DefinitionId)
        {
            ProductDetailField productDetailField = context.ProductDetailFields.First(n => n.Id == ProductDetailFieldId);

            productDetailField.DefinitionId = DefinitionId;

            context.ProductDetailFields.Update(productDetailField);

            context.SaveChanges();

            return new EmptyResult();
        }*/

        [HttpGet]
        public ViewResult Product(int productId)
        {
            return View();
        }
        [HttpPost]
        public IActionResult ProductGetInfo(int productId)
        {
            Product product = _context.Products.Include(n => n.ProductDetailFields)
                .ThenInclude(n => n.DetailField)
                .ThenInclude(n => n.Definitions)
                .Include(n => n.Category)
                .FirstOrDefault(n => n.Id == productId);

            if (product == null)
                return new EmptyResult();

            return Content(JsonConvert.SerializeObject(product));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult EditSave(EditSaveModel model)
        {
            if (ModelState.IsValid)
            {
                // Если картинка добавлена, то...
                if (model.FileImg != null)
                {
                    // Заливаем ее на сервер
                    model.FileImg.CopyTo(new FileStream($"{_hostEnv.WebRootPath}/images/Products/{model.FileImg.FileName}", FileMode.Create));

                    // Меняем на новый путь
                    model.Product.ImgPath = $"/images/Products/{model.FileImg.FileName}";
                }

                // Если добавлено хотя бы одно поле
                if (model.ProductDetailFieldId != null)
                {
                    int fieldsCount = model.ProductDetailFieldId.Count();

                    // Проходим каждое добавленное поле и его определение
                    for (int i = 0; i < fieldsCount; i++)
                    {
                        ProductDetailField productDetailField = _context.ProductDetailFields
                            .FirstOrDefault(n => n.Id == model.ProductDetailFieldId[i]);

                        // Если такое поле нашлось в бд
                        if (productDetailField != null)
                        {
                            // Сохраняем новое определение у поля в данном продукте
                            productDetailField.DefinitionId = model.DefinitionId[i];

                            _context.ProductDetailFields.Update(productDetailField);
                        }
                    }
                }

                _context.Products.Update(model.Product);
                _context.SaveChanges();
            }

            return RedirectToAction("Edit", "Home", new { Id = model.Product.Id });
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ViewResult Add()
        {
            return View(new AddViewModel { Product = null, FileImg = null, Categories = _context.Categories.ToList() });
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Add(AddViewModel avm)
        {
            if (!ModelState.IsValid)
                return View(avm);

            string fileName;

            fileName = $"{_hostEnv.WebRootPath}/images/Products/{avm.FileImg.FileName}";

            avm.FileImg.CopyTo(new FileStream($"{_hostEnv.WebRootPath}/images/Products/{avm.FileImg.FileName}", FileMode.Create));

            avm.Product.ImgPath = $"/images/Products/{avm.FileImg.FileName}";

            _context.Products.Add(avm.Product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public EmptyResult RemoveProduct(int id)
        {
            Product product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return new EmptyResult();
        }

        [HttpGet]
        public ViewResult CartChangeProductNum()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CartChangeProductNum(int id, int count)
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            user = _userManager.Users.Where(u => u.Id == user.Id).Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts)
                .ThenInclude(u => u.Product)
                .FirstOrDefault();

            CartProduct cartProduct = user.Cart.CartProducts.FirstOrDefault(n => n.ProductId == id);

            cartProduct.Count = count;

            await _userManager.UpdateAsync(user);

            return Content(JsonConvert.SerializeObject(cartProduct.Product.Count));
        }

        private bool IsSimilar(string searchArgument, string productName)
        {
            string[] searchArgumentWords = searchArgument.Split(' ');
            string[] productNameWords = productName.Split(' ');

            for (int i = 0; i < searchArgumentWords.Count(); i++)
            {
                if (productNameWords.ElementAtOrDefault(i) == null)
                    return false;

                if (searchArgumentWords[i] != productNameWords[i])
                    return false;
            }

            return true;
        }
        /*private double CompareTwoString(string strFirst, string strSecond)
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
        }*/

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
