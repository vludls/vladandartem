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
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace vladandartem.Controllers
{
    [Route("AdminMenu")]
    [Authorize(Roles = "admin")]
    public class AdminMenuController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ProductContext _context;

        public AdminMenuController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, ProductContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        /*******************************************
         * Categories
         *******************************************/
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("Categories")]
        [HttpGet]
        public ViewResult Main()
        {
            return View();
        }

        /// <summary>
        /// Получение категорий и их секций
        /// </summary>
        /// <response code="200">
        /// Возвращает JSON:
        /// {
        ///     "Sections" = [],
        ///     "Categories" = []
        /// }
        /// </response>
        [Route("Categories/Api/Get")]
        [HttpPost]
        public ContentResult MainGet()
        {
            MainViewModel viewModel = new MainViewModel
            {
                Sections = _context.Sections.ToList(),
                Categories = _context.Categories.ToList()
            };

            return Content(JsonConvert.SerializeObject(viewModel));
        }

        /// <summary>
        /// Добавление новой категории
        /// </summary>
        /// <param name="sectionId">Id секции</param>
        /// <param name="categoryName">Название категории</param>
        /// <returns></returns>
        [Route("Categories/Api/Add")]
        [HttpPost]
        public IActionResult AddCategory([Required]int sectionId, [Required]string categoryName)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

            if (_context.Sections.FirstOrDefault(s => s.Id == sectionId) == null)
                return new EmptyResult();

            Category category = new Category { Name = categoryName, SectionId = sectionId };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return Content(JsonConvert.SerializeObject(category));
        }

        /// <summary>
        /// Удаление категории
        /// </summary>
        /// <param name="categoryId">Id категории</param>
        /// <returns></returns>
        [Route("Categories/Api/Delete")]
        [HttpPost]
        public IActionResult DeleteCategory([Required]int categoryId)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

            Category category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
                return new EmptyResult();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Content(JsonConvert.SerializeObject(new { CategoryId = categoryId }));
        }

        /*******************************************
         * Users
         *******************************************/

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("Users")]
        [HttpGet]
        public IActionResult Users()
        {
            return View();
        }

        /// <summary>
        /// Получение всех юзеров
        /// </summary>
        /// <returns></returns>
        [Route("Users/Api/Get")]
        [HttpPost]
        public IActionResult GetUsers()
        {
            return Content(JsonConvert.SerializeObject(_userManager.Users.ToList()));
        }

        /*******************************************
         * User
         *******************************************/

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("User")]
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("User/Edit")]
        [HttpGet]
        public async Task<IActionResult> EditUser([Required]int id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            EditUserViewModel viewModel = new EditUserViewModel
            {
                Email = user.Email,
                Year = user.Year,
                UserRoles = await _userManager.GetRolesAsync(user),
                Roles = _roleManager.Roles.ToList()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <param name="cuvm"></param>
        /// <returns></returns>
        [Route("User/Api/Add")]
        [HttpPost]
        public async Task<IActionResult> AddUser(CreateUserViewModel cuvm)
        {
            User user = new User { Email = cuvm.Email, UserName = cuvm.Email, Year = cuvm.Year, };

            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, cuvm.Password);

                if (result.Succeeded)
                {
                    Cart cart = new Cart { UserId = user.Id };

                    _context.Cart.Add(cart);
                    _context.SaveChanges();

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

        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <param name="email">E-mail</param>
        /// <param name="year">Дата рождения</param>
        /// <param name="roles">Роли</param>
        /// <returns></returns>
        [Route("User/Api/Save")]
        [HttpPost]
        public async Task<IActionResult> UserSave([Required]int id, [Required]string email, [Required]string year, List<string> roles)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                user.Email = email;
                user.UserName = email;
                user.Year = year;

                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);

                // получаем все роли
                var allRoles = _roleManager.Roles.ToList();

                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                var result = await _userManager.UpdateAsync(user);

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

            return Content("Такого юзера не существует");
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <returns></returns>
        [Route("User/Api/Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            User mainUser = await _userManager.GetUserAsync(HttpContext.User);

            User findedUser = await _userManager.FindByIdAsync(Convert.ToString(id));

            if (findedUser == null || mainUser == findedUser)
                return Content(JsonConvert.SerializeObject(new { UserId = 0 }));

            findedUser = _userManager.Users.Where(u => u.Id == findedUser.Id)
                .Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts)
                .ThenInclude(u => u.Product)
                .FirstOrDefault();

            _context.Cart.Remove(findedUser.Cart);

            _context.SaveChanges();

            await _userManager.DeleteAsync(findedUser);

            return Content(JsonConvert.SerializeObject(new { UserId = id }));
        }

        /*******************************************
         * Analytics
         *******************************************/

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("Analytics")]
        [HttpGet]
        public ViewResult Analytics()
        {
            return View();
        }

        /// <summary>
        /// Получение данных для полей в аналитике
        /// </summary>
        /// <returns></returns>
        [Route("Analytics/Api/GetAnalyticsFields")]
        [HttpPost]
        public ContentResult GetAnalyticsField()
        {
            AnalyticsViewModel model = new AnalyticsViewModel
            {
                Categories = _context.Categories.ToList(),
                Products = _context.Products.ToList(),
                Users = _context.Users.ToList()
            };

            return Content(JsonConvert.SerializeObject(model));
        }

        /// <summary>
        /// Получение списка продуктов по категории
        /// </summary>
        /// <param name="categoryId">Id категории</param>
        /// <returns></returns>
        [Route("Analytics/Api/GetCategoryProducts")]
        [HttpPost]
        public IActionResult AnalyticsLoadProductsOfChoosedCategory([Required]int categoryId)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

            if (_context.Categories.Any(n => n.Id == categoryId))
            {
                Category category = _context.Categories.Include(n => n.Products)
                    .FirstOrDefault(n => n.Id == categoryId);

                return Content(JsonConvert.SerializeObject(category.Products));
            }
            else
            {
                return Content(JsonConvert.SerializeObject(_context.Products.ToList()));
            }
        }

        /// <summary>
        /// Получение общей аналитики
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Analytics/Api/GetGeneralAnalytics")]
        [HttpPost]
        public IActionResult LoadGeneralAnalytics(LoadAnalytics model)
        {
            // Получаем все заказы и их поля
            var orders = _context.Orders.Include(n => n.CartProducts)
                .ThenInclude(n => n.Product)
                .ThenInclude(c => c.Category)
                .Include(n => n.User)
                .ToList();

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

            var grouping1 = products.GroupBy(cartProduct => cartProduct.Order.OrderTime.Year).OrderBy(yearGroup => yearGroup.Key)
                .Select(r => new
                {
                    Year = r.Key,
                    Months = r.GroupBy(cartProduct => cartProduct.Order.OrderTime.Month).OrderBy(monthGroup => monthGroup.Key)
                    .Select(c => new
                    {
                        Month = c.Key,
                        Days = c.GroupBy(cartProduct => cartProduct.Order.OrderTime.Day).OrderBy(dayGroup => dayGroup.Key)
                        .Select(m => new
                        {
                            Day = m.Key,
                            Sales = m.Sum(cp => cp.Count),
                            Revenue = m.Sum(cp => cp.Count + cp.Product.Price)
                        }).ToList()
                    })
                }).ToList();

            /*DateTime datePostBuff = products.First().Order.OrderTime;

            LoadGeneralAnalyticsViewModel productAnalytics = new LoadGeneralAnalyticsViewModel();

            productAnalytics.MonthsState.Add(new MonthState(products.First().Order.OrderTime));

            // Проходим все отфильтрованные продукты (CartProduct)
            foreach (var product in products)
            {
                // Если прошлый день больше следующего
                if (datePostBuff.Day > product.Order.OrderTime.Day)
                {
                    // Добавляем месяц к продукту в аналитике
                    productAnalytics.MonthsState.Add(new MonthState(datePostBuff));
                }

                // Прошлая дата
                datePostBuff = product.Order.OrderTime;

                var month = productAnalytics.MonthsState.Last();

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

                    productAnalytics.Sales += product.Count;
                    productAnalytics.Revenue += product.Count * product.Product.Price;

                    month.Days.Add(day);
                }
                else
                {
                    day.Sales += product.Count;
                    day.Revenue += product.Count * product.Product.Price;

                    productAnalytics.Sales += product.Count;
                    productAnalytics.Revenue += product.Count * product.Product.Price;
                }
            }
            */
            return Content(JsonConvert.SerializeObject(grouping1));
        }

        [Route("Analytics/Api/GetAnalytics")]
        [HttpPost]
        public IActionResult LoadAnalytics(LoadAnalytics model)
        {
            // Получаем все заказы и их поля
            var orders = _context.Orders.Include(n => n.CartProducts)
                .ThenInclude(n => n.Product)
                .ThenInclude(c => c.Category)
                .Include(n => n.User)
                .ToList();

            var products = from order in orders
                           from cartProduct in order.CartProducts
                           where order.IsPaid
                           orderby cartProduct.Id
                           select cartProduct;

            // Если не отображать за все время, то фильтрует по выбранному периоду даты
            if (model.AllTime != 1)
                products = products.Where(cp =>
                    cp.Order.OrderTime.Day >= model.DateFrom.Day &&
                    cp.Order.OrderTime.Month >= model.DateFrom.Month &&
                    cp.Order.OrderTime.Year >= model.DateFrom.Year &&
                    cp.Order.OrderTime.Day <= model.DateTo.Day &&
                    cp.Order.OrderTime.Month <= model.DateTo.Month &&
                    cp.Order.OrderTime.Year <= model.DateTo.Year
                );

            // Если выбрана конкретная категория, то фильтруем по ней
            if (model.CategoryId != 0)
                products = products.Where(n => n.Product.CategoryId == model.CategoryId);

            // Если выбран конкретный продукт, то фильтруем по нему
            if (model.ProductId != 0)
                products = products.Where(n => n.Product.Id == model.ProductId);

            // Если выбран конкретный пользователь, то фильтруем по нему
            if (model.UserId != 0)
                products = products.Where(n => n.Order.User.Id == model.UserId);

            products = products.OrderBy(n => n.Order.OrderTime);
            
            var grouping1 = products.GroupBy(n => n.Product)
                .Select(productGroup => new
                {
                    Product = productGroup.Key,
                    Years = productGroup.GroupBy(cartProduct => cartProduct.Order.OrderTime.Year).OrderBy(yearGroup => yearGroup.Key)
                    .Select(r => new
                    {
                        Year = r.Key,
                        Months = r.GroupBy(cartProduct => cartProduct.Order.OrderTime.Month).OrderBy(monthGroup => monthGroup.Key)
                        .Select(c => new
                        {
                            Month = c.Key,
                            Days = c.GroupBy(cartProduct => cartProduct.Order.OrderTime.Day).OrderBy(dayGroup => dayGroup.Key)
                            .Select(m => new
                            {
                                Day = m.Key,
                                Sales = m.Sum(cp => cp.Count),
                                Revenue = m.Sum(cp => cp.Count) * productGroup.Key.Price
                            }).ToList()
                        })
                    }).ToList()
                }).ToList();

            return Content(JsonConvert.SerializeObject(grouping1));
        }

        /*******************************************
         * Section
         *******************************************/

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("Section")]
        [HttpGet]
        public ViewResult Section()
        {
            return View();
        }

        /// <summary>
        /// Получение всех секций
        /// </summary>
        /// <returns></returns>
        [Route("Section/Api/GetAll")]
        [HttpPost]
        public ContentResult GetSections()
        {
            return Content(JsonConvert.SerializeObject(_context.Sections.ToList()));
        }

        /// <summary>
        /// Добавление секции
        /// </summary>
        /// <param name="sectionName">Название секции</param>
        /// <returns></returns>
        [Route("Section/Api/Add")]
        [HttpPost]
        public IActionResult AddSection([Required]string sectionName)
        {
            if (ModelState.IsValid)
            {
                Section section = new Section { Name = sectionName };

                _context.Sections.Add(section);

                _context.SaveChanges();

                return Content(JsonConvert.SerializeObject(section));
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Удаление секции
        /// </summary>
        /// <param name="sectionId">Id секции</param>
        /// <returns></returns>
        [Route("Section/Api/Delete")]
        [HttpPost]
        public IActionResult SectionDelete([Required]int sectionId)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

            Section section = _context.Sections.FirstOrDefault(s => s.Id == sectionId);

            if (section == null)
                return new EmptyResult();

            _context.Sections.Remove(section);
            _context.SaveChanges();

            return Content(JsonConvert.SerializeObject(new { SectionId = sectionId }));
        }

        /*******************************************
         * Role
         *******************************************/
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("Role")]
        [HttpGet]
        public IActionResult Role()
        {
            return View(_roleManager.Roles.ToList());
        }

        /*******************************************
         * DetailField
         *******************************************/
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("DetailField")]
        [HttpGet]
        public IActionResult DetailField()
        {
            return View();
        }

        /// <summary>
        /// Получение всех детальных полей
        /// </summary>
        /// <returns></returns>
        [Route("DetailField/GetAll")]
        [HttpPost]
        public IActionResult GetDetailFields()
        {
            return Content(JsonConvert.SerializeObject(_context.DetailFields.Include(n => n.Definitions).ToList()));
        }

        /// <summary>
        /// Добавление детального поля
        /// </summary>
        /// <param name="detailFieldName">Название детального поля</param>
        /// <returns></returns>
        [Route("DetailField/Api/Add")]
        [HttpPost]
        public IActionResult DetailFieldAdd([Required]string detailFieldName)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

            if (_context.DetailFields.Any(n => n.Name == detailFieldName))
                return new EmptyResult();

            DetailField detailField = new DetailField { Name = detailFieldName, Definitions = new List<Definition>() };
            _context.DetailFields.Add(detailField);

            _context.SaveChanges();

            return Content(JsonConvert.SerializeObject(detailField));
        }

        /// <summary>
        /// Удаление детального поля
        /// </summary>
        /// <param name="detailFieldId">Id детального поля</param>
        /// <returns></returns>
        [Route("DetailField/Api/Delete")]
        [HttpPost]
        public IActionResult DetailFieldDelete([Required]int detailFieldId)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

            DetailField detailField = _context.DetailFields.Find(detailFieldId);

            if (detailField == null)
                return new EmptyResult();

            _context.DetailFields.Remove(detailField);
            _context.SaveChanges();

            return Content(JsonConvert.SerializeObject(new { DetailFieldId = detailFieldId }));
        }

        /// <summary>
        /// Добавление определения для детального поля
        /// </summary>
        /// <param name="detailFieldId">Id детального поля</param>
        /// <param name="definitionName">Название определения</param>
        /// <returns></returns>
        [Route("DetailField/Definition/Api/Add")]
        [HttpPost]
        public IActionResult DetailFieldDefinitionAdd([Required]int detailFieldId, [Required]string definitionName)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

                DetailField detailField = _context.DetailFields.Include(n => n.Definitions).FirstOrDefault(n => n.Id == detailFieldId);

            if (detailField == null)
                return new EmptyResult();

            if (detailField.Definitions.Any(n => n.Name == definitionName))
                return new EmptyResult();

            Definition definition = new Definition { Name = definitionName, DetailFieldId = detailFieldId };

            _context.Definitions.Add(definition);
            _context.SaveChanges();

            return Content(JsonConvert.SerializeObject(new { DetailFieldId = detailFieldId, Id = definition.Id, Name = definition.Name }));
        }

        /// <summary>
        /// Удаление определения у детального поля
        /// </summary>
        /// <param name="definitionId">Id определения</param>
        /// <returns></returns>
        [Route("DetailField/Definition/Api/Delete")]
        [HttpPost]
        public IActionResult DetailFieldDefinitionDelete([Required]int definitionId)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

            Definition definition = _context.Definitions.Find(definitionId);

            if (definition == null)
                return new EmptyResult();

            var productDetailFields = _context.ProductDetailFields.Where(n => n.DefinitionId == definitionId);

            foreach (var productDetailField in productDetailFields)
            {
                productDetailField.DefinitionId = null;
            }

            _context.Definitions.Remove(definition);
            _context.SaveChanges();

            return Content(JsonConvert.SerializeObject(new { DefinitionId = definitionId }));
        }

        /// <summary>
        /// Получение списка продуктов с указанным определением
        /// </summary>
        /// <param name="definitionId">Id определения</param>
        /// <returns></returns>
        [Route("DetailField/Definition/Api/GetProducts")]
        [HttpPost]
        public ContentResult ProductsOfDefinition(int definitionId)
        {
            return Content(JsonConvert.SerializeObject(_context.ProductDetailFields.Include(pdf => pdf.Product)
                .Where(pdf => pdf.DefinitionId == definitionId)
                .Select(pdf => new { Id = pdf.Product.Id, Name = pdf.Product.Name })));
        }
    }
}
