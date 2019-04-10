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
        public async Task<IActionResult> Main()
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            return View(context.Categories.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string CategoryName)
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            context.Categories.Add(new Category { Name = CategoryName });

            context.SaveChanges();

            return Redirect("~/AdminMenu/Main");
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
        public async Task<IActionResult> EditUser(string id)
        {
            if (await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            User user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel euvm = new EditUserViewModel { Email = user.Email, Year = user.Year };

            return View();
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
        public IActionResult LoadAnalytics(SomeViewModel model)
        {
            // Получаем все заказы и их поля
            var buff = context.Orders.Include(n => n.CartProducts)
                .ThenInclude(n => n.Product)
                .ThenInclude(c => c.Category)
                .Include(n => n.User)
                .ToList();

            List<Test2> test = new List<Test2>();

            // Текущая дата(счетчик даты)
            DateTime dateFromBuff = model.DateFrom;
            // Прошлая текущая дата
            DateTime dateFromPostBuff = dateFromBuff;

            // Проходим каждый день с указанной начальной даты до указаной конечной даты
            while (DateTime.Compare(dateFromBuff, model.DateTo) <= 0)
            {
                dateFromPostBuff = dateFromPostBuff.AddDays(1);

                // Получаем продукты(CartProduct), которые соответствуют текущему счетчику даты 
                var products = from order in buff
                               where order.OrderTime.Day == dateFromBuff.Day &&
                                   order.OrderTime.Month == dateFromBuff.Month &&
                                   order.OrderTime.Year == dateFromBuff.Year
                               from ttt in order.CartProducts
                               orderby ttt.Id
                               select ttt;

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

                if (model.UserEmail != "0")
                {
                    products = products.Where(n => n.Order.User.Email == model.UserEmail);
                }

                // Проходим отфильтрованные продукты
                foreach (var product in products)
                {
                    var element = test.FirstOrDefault(n => n.Product.Id == product.Product.Id);

                    // Если элемента в массиве результата нет
                    if (element == null)
                    {
                        element = new Test2 { Product = product.Product };
                        
                        test.Add(element);
                    }

                    // Если прошлый день больше следующего
                    if (dateFromPostBuff.Day >= dateFromBuff.Day)
                    {
                        // Добавляем месяц
                        element.MonthsState.Add(new MonthState(dateFromBuff));
                    }

                    var month = element.MonthsState.Last();

                    var day = new DayState(dateFromBuff);
                    day.Sales += product.Count;
                    day.Revenue += product.Count * product.Product.Price;

                    element.Sales += day.Sales;
                    element.Revenue += day.Revenue;

                    month.Days.Add(day);
                }

                dateFromBuff = dateFromBuff.AddDays(1);
            }
            
            return Content(JsonConvert.SerializeObject(test));
        }
    }
}
