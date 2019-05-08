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
using vladandartem.Models.ViewModels.AdminMenu;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using vladandartem.Models.Request.AdminMenu;
using vladandartem.Data.Models;
using AutoMapper;

namespace vladandartem.Controllers
{
    [Route("adminmenu")]
    [Authorize(Roles = "admin")]
    public class AdminMenuController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ProductContext _context;
        private readonly IMapper _mapper;

        public AdminMenuController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager,
            ProductContext context, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
        }

        /*******************************************
         * Categories
         *******************************************/
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("categories")]
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
        [Route("categories/api/get")]
        [HttpPost]
        public JsonResult MainGet()
        {
            MainViewModel viewModel = new MainViewModel
            {
                Sections = _context.Sections.ToList(),
                Categories = _context.Categories.ToList()
            };

            return new JsonResult(viewModel);
        }

        /// <summary>
        /// Добавление новой категории
        /// </summary>
        /// <param name="sectionId">Id секции</param>
        /// <param name="categoryName">Название категории</param>
        /// <returns></returns>
        [Route("categories/api/add")]
        [HttpPost]
        public IActionResult AddCategory([Required]int sectionId, [Required]string categoryName)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

            if (_context.Sections.Any(s => s.Id == sectionId))
                return new EmptyResult();

            Category category = new Category { Name = categoryName, SectionId = sectionId };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return new JsonResult(category);
        }

        /// <summary>
        /// Удаление категории
        /// </summary>
        /// <param name="categoryId">Id категории</param>
        /// <returns></returns>
        [Route("categories/api/delete")]
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

            return new JsonResult(new { CategoryId = categoryId });
        }

        /*******************************************
         * Users
         *******************************************/

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("users")]
        [HttpGet]
        public IActionResult Users()
        {
            return View();
        }

        /// <summary>
        /// Получение всех юзеров
        /// </summary>
        /// <returns></returns>
        [Route("users/api/get")]
        [HttpPost]
        public JsonResult GetUsers()
        {
            // Можно ли пройти каждого юзера через .Select и выбрать нужные данные без mapping'a
            return new JsonResult(new { Users = _mapper.Map<List<User>>(_userManager.Users.ToList()) });
        }

        /*******************************************
         * User
         *******************************************/

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("user")]
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("user/edit")]
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
        [Route("user/api/add")]
        [HttpPost]
        public async Task<IActionResult> AddUser(CreateUserViewModel cuvm)
        {
            if (ModelState.IsValid)
            {
                User user = _mapper.Map<User>(cuvm);

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

        [Route("user/api/save")]
        [HttpPost]
        public async Task<IActionResult> UserSave(UserSaveModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id.ToString());

                if (user != null)
                {
                    Mapper.Map(model, user);
                    //user.Email = model.Email;
                    //user.UserName = model.Email;
                    //user.Year = model.Year;

                    // получем список ролей пользователя
                    var userRoles = await _userManager.GetRolesAsync(user);

                    // получаем все роли
                    var allRoles = _roleManager.Roles.ToList();

                    var addedRoles = model.Roles.Except(userRoles);
                    var removedRoles = userRoles.Except(model.Roles);

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
            }

            return Content("Такого юзера не существует");
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <returns></returns>
        [Route("user/api/delete")]
        [HttpPost]
        public async Task<JsonResult> DeleteUser(int id)
        {
            User mainUser = await _userManager.GetUserAsync(HttpContext.User);

            User findedUser = await _userManager.FindByIdAsync(id.ToString());

            if (findedUser == null || mainUser == findedUser)
                return new JsonResult(new { UserId = 0 });

            findedUser = _userManager.Users.Where(u => u.Id == findedUser.Id)
                .Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts)
                .ThenInclude(u => u.Product)
                .FirstOrDefault();

            _context.Cart.Remove(findedUser.Cart);
            _context.SaveChanges();

            await _userManager.DeleteAsync(findedUser);

            return new JsonResult(new { UserId = id });
        }

        /*******************************************
         * Analytics
         *******************************************/

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("analytics")]
        [HttpGet]
        public ViewResult Analytics()
        {
            return View();
        }

        /// <summary>
        /// Получение данных для полей в аналитике
        /// </summary>
        /// <returns></returns>
        [Route("analytics/api/getanalyticsfields")]
        [HttpPost]
        public JsonResult GetAnalyticsField()
        {
            AnalyticsViewModel viewModel = new AnalyticsViewModel
            {
                Categories = _context.Categories.ToList(),
                Products = _context.Products.ToList(),
                Users = _mapper.Map<List<User>>(_context.Users.ToList())
            };

            return new JsonResult(viewModel);
        }

        /// <summary>
        /// Получение списка продуктов по категории
        /// </summary>
        /// <param name="categoryId">Id категории</param>
        /// <returns></returns>
        [Route("analytics/api/getcategoryproducts")]
        [HttpPost]
        public IActionResult AnalyticsLoadProductsOfChoosedCategory([Required]int categoryId)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

            IQueryable<Product> products = _context.Products;

            if (categoryId > 0)
                products = products.Where(x => x.CategoryId == categoryId);

            return new JsonResult(new
            {
                Products = _mapper.Map<AnalyticsLoadProductsOfChoosedCategoryViewModel>(products.ToList())
            });
        }

        /// <summary>
        /// Получение общей аналитики
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("analytics/api/getgeneralanalytics")]
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

            var group = products.GroupBy(cartProduct => cartProduct.Order.OrderTime.Year).OrderBy(yearGroup => yearGroup.Key)
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

            return new JsonResult(group);
        }

        [Route("analytics/api/getanalytics")]
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

            var group = products.GroupBy(n => n.Product)
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

            return new JsonResult(group);
        }

        /*******************************************
         * Section
         *******************************************/

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("section")]
        [HttpGet]
        public ViewResult Section()
        {
            return View();
        }

        /// <summary>
        /// Получение всех секций
        /// </summary>
        /// <returns></returns>
        [Route("section/api/getall")]
        [HttpPost]
        public JsonResult GetSections()
        {
            return new JsonResult(_context.Sections.ToList());
        }

        /// <summary>
        /// Добавление секции
        /// </summary>
        /// <param name="sectionName">Название секции</param>
        /// <returns></returns>
        [Route("section/api/add")]
        [HttpPost]
        public IActionResult AddSection([Required]string sectionName)
        {
            if (!ModelState.IsValid)
                return new EmptyResult();

            Section section = new Section { Name = sectionName };

            _context.Sections.Add(section);
            _context.SaveChanges();

            return new JsonResult(section);
        }

        /// <summary>
        /// Удаление секции
        /// </summary>
        /// <param name="sectionId">Id секции</param>
        /// <returns></returns>
        [Route("section/api/delete")]
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

            return new JsonResult(new { SectionId = sectionId });
        }

        /*******************************************
         * Role
         *******************************************/
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("role")]
        [HttpGet]
        public IActionResult Role()
        {
            return View(_roleManager.Roles.ToList());
        }

        /*******************************************
         * DetailField
         *******************************************/
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("detailfield")]
        [HttpGet]
        public IActionResult DetailField()
        {
            return View();
        }

        /// <summary>
        /// Получение всех детальных полей
        /// </summary>
        /// <returns></returns>
        [Route("detailfield/getall")]
        [HttpPost]
        public JsonResult GetDetailFields()
        {
            return new JsonResult(_context.DetailFields.Include(n => n.Definitions)
                .Select(f => new
                {
                    DetailFieldId = f.Id,
                    Name = f.Name,
                    Definitions = f.Definitions.Select(d => new
                    {
                        DefinitionId = d.Id,
                        Name = d.Name
                    }).ToList()
                }).ToList());
        }

        /// <summary>
        /// Добавление детального поля
        /// </summary>
        /// <param name="detailFieldName">Название детального поля</param>
        /// <returns></returns>
        [Route("detailfield/api/add")]
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

            return new JsonResult(new { DetailFieldId = detailField.Id, detailField.Name, detailField.Definitions });
        }

        /// <summary>
        /// Удаление детального поля
        /// </summary>
        /// <param name="detailFieldId">Id детального поля</param>
        /// <returns></returns>
        [Route("detailfield/api/delete")]
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

            return new JsonResult(new { DetailFieldId = detailFieldId });
        }

        /// <summary>
        /// Добавление определения для детального поля
        /// </summary>
        /// <param name="detailFieldId">Id детального поля</param>
        /// <param name="definitionName">Название определения</param>
        /// <returns></returns>
        [Route("detailfield/definition/api/add")]
        [HttpGet]
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

            return new JsonResult(new { DetailFieldId = detailFieldId, Id = definition.Id, Name = definition.Name });
        }

        /// <summary>
        /// Удаление определения у детального поля
        /// </summary>
        /// <param name="definitionId">Id определения</param>
        /// <returns></returns>
        [Route("detailfield/definition/api/delete")]
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

            return new JsonResult(new { DefinitionId = definitionId });
        }

        /// <summary>
        /// Получение списка продуктов с указанным определением
        /// </summary>
        /// <param name="definitionId">Id определения</param>
        /// <returns></returns>
        [Route("detailfield/definition/api/getproducts")]
        [HttpPost]
        public JsonResult ProductsOfDefinition(int definitionId)
        {
            return new JsonResult(_context.ProductDetailFields.Include(pdf => pdf.Product)
                .Where(pdf => pdf.DefinitionId == definitionId)
                .Select(pdf => new { Id = pdf.Product.Id, Name = pdf.Product.Name }).ToList());
        }
    }
}
