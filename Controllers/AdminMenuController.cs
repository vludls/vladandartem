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
using vladandartem.ViewModels.AdminMenu;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace vladandartem.Controllers
{
    public class AdminMenuController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ProductContext context;

        public AdminMenuController(UserManager<User> userManager, ProductContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Main()
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            MainViewModel mvm = new MainViewModel {
                Sections = context.Sections.ToList(),
                Categories = context.Categories.ToList()
            };

            return View(mvm);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(int SectionId, string CategoryName)
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            Category category = new Category { Name = CategoryName, SectionId = SectionId };

            context.Categories.Add(category);

            context.SaveChanges();

            return Content(JsonConvert.SerializeObject(category));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            Category SomeCategory = context.Categories.Find(id);

            if (SomeCategory == null)
            {
                return NotFound();
            }


            context.Categories.Remove(SomeCategory);

            context.SaveChanges();

            return Redirect("~/AdminMenu/Main");
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            return View(userManager.Users.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel cuvm)
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            User user = new User { Email = cuvm.Email, UserName = cuvm.Email, Year = cuvm.Year, };

            if (ModelState.IsValid)
            {
                var result = await userManager.CreateAsync(user, cuvm.Password);

                if (result.Succeeded)
                {
                    Cart cart = new Cart { UserId = user.Id };

                    context.Cart.Add(cart);

                    context.SaveChanges();

                    return RedirectToAction("Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(cuvm);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            User user = await userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel euvm = new EditUserViewModel { Email = user.Email, Year = user.Year };

            return View(euvm);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel euvm)
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            User user = await userManager.FindByIdAsync(euvm.Id);

            if (user != null)
            {
                user.Email = euvm.Email;
                user.UserName = euvm.Email;
                user.Year = euvm.Year;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(euvm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            User user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                var buff = userManager.Users.Where(u => u.Id == user.Id)
                    .Include(u => u.Cart)
                    .ThenInclude(u => u.CartProducts)
                    .ThenInclude(u => u.Product)
                    .FirstOrDefault();

                context.Cart.Remove(buff.Cart);

                context.SaveChanges();

                await userManager.DeleteAsync(user);
            }

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult Analytics()
        {
            AnalyticsViewModel model = new AnalyticsViewModel
            {
                Categories = context.Categories.ToList(),
                Products = context.Products.ToList(),
                Users = context.Users.ToList()
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult LoadGeneralAnalytics(LoadAnalytics model)
        {
            return Content(getAnalyticsJson(model, true));
        }

        [HttpPost]
        public IActionResult LoadAnalytics(LoadAnalytics model)
        {
            return Content(getAnalyticsJson(model, false));
        }

        [HttpGet]
        public IActionResult Section()
        {
            return View(context.Sections.ToList());
        }
        [HttpPost]
        public IActionResult SectionAdd(string SectionName)
        {
            context.Sections.Add(new Section { Name = SectionName });

            context.SaveChanges();

            return new EmptyResult();
        }
        [HttpPost]
        public IActionResult SectionDelete(int SectionId)
        {
            Section section = context.Sections.Find(SectionId);

            if(section != null)
            {
                context.Sections.Remove(section);

                context.SaveChanges();
            }

            return new EmptyResult();
        }

        private string getAnalyticsJson(LoadAnalytics model, bool isSkip)
        {
            // Получаем все заказы и их поля
            var orders = context.Orders.Include(n => n.CartProducts)
                .ThenInclude(n => n.Product)
                .ThenInclude(c => c.Category)
                .Include(n => n.User)
                .ToList();

            List<ProductAnalytics> productAnalytics = new List<ProductAnalytics>();

            // Получаем все CartProduct и сортируем по возрастанию по айди
            var products = from order in orders
                           from cartProduct in order.CartProducts
                           orderby cartProduct.Id
                           select cartProduct;

            // Если не отображать за все время, то фильтрует по выбранной дате
            if (model.AllTime != 1)
            {
                products = products.Where(cp =>
                    cp.Order.OrderTime.Day >= model.DateFrom.Day &&
                    cp.Order.OrderTime.Month >= model.DateFrom.Month &&
                    cp.Order.OrderTime.Year >= model.DateFrom.Year &&
                    cp.Order.OrderTime.Day <= model.DateTo.Day &&
                    cp.Order.OrderTime.Month <= model.DateTo.Month &&
                    cp.Order.OrderTime.Year <= model.DateTo.Year
                );
            }

            // Если isSkip == false, то значит это подгрузка общей статистики и нам надо получить все элементы, а не только 10
            if(isSkip)
                products = products.Skip(model.LastItemId).Take(10);

            // Если выбрана конкретная категория, то фильтруем по ней
            if (model.CategoryId != 0)
            {
                products = products.Where(n => n.Product.CategoryId == model.CategoryId);
            }

            // Если выбран конкретный продукт, то фильтруем по нему
            if (model.ProductId != 0)
            {
                products = products.Where(n => n.Product.Id == model.ProductId);
            }

            // Если выбран конкретный пользователь, то фильтруем по нему
            if (model.UserId != 0)
            {
                products = products.Where(n => n.Order.User.Id == model.UserId);
            }

            products = products.OrderBy(n => n.Order.OrderTime);

            DateTime datePostBuff = new DateTime();

            // Проходим все отфильтрованные продукты (CartProduct)
            foreach (var product in products)
            {
                // Ищем в готовой аналитике этот продукт
                var lavmItem = productAnalytics.FirstOrDefault(item => item.Product.Id == product.Product.Id);

                // Если его там нет, то заносим в аналитику этот продукт
                if (lavmItem == null)
                {
                    lavmItem = new ProductAnalytics { Product = product.Product };

                    lavmItem.MonthsState.Add(new MonthState(product.Order.OrderTime));

                    productAnalytics.Add(lavmItem);
                }

                // Если прошлый день больше следующего
                if (datePostBuff.Day > product.Order.OrderTime.Day)
                {
                    // Добавляем месяц к продукту в аналитике
                    lavmItem.MonthsState.Add(new MonthState(datePostBuff));
                }

                // Прошлая дата
                datePostBuff = product.Order.OrderTime;

                var month = lavmItem.MonthsState.Last();

                DayState day = month.Days.Find(ds =>
                    ds.Day.Day == datePostBuff.Day &&
                    ds.Day.Month == datePostBuff.Month &&
                    ds.Day.Year == datePostBuff.Year
                );

                if (day == null)
                {
                    day = new DayState(datePostBuff);

                    day.Sales += product.Count;
                    day.Revenue += product.Count * product.Product.Price;

                    lavmItem.Sales += product.Count;
                    lavmItem.Revenue += product.Count * product.Product.Price;

                    month.Days.Add(day);
                }
                else
                {
                    day.Sales += product.Count;
                    day.Revenue += product.Count * product.Product.Price;

                    lavmItem.Sales += product.Count;
                    lavmItem.Revenue += product.Count * product.Product.Price;
                }
            }

            return JsonConvert.SerializeObject(productAnalytics);
        }
    }
}
