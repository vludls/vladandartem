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
            if(await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            return View(context.Categories.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(string CategoryName)
        {
            if(await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            context.Categories.Add(new Category{ Name = CategoryName });

            context.SaveChanges();
            
            return Redirect("~/AdminMenu/Main");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if(await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            Category SomeCategory = context.Categories.Find(id);

            if(SomeCategory == null)
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
            if(await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            return View(userManager.Users.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            if(await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel cuvm)
        {
            if(await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            User user = new User { Email = cuvm.Email, UserName = cuvm.Email, Year = cuvm.Year,  };
            
            if(ModelState.IsValid)
            {
                var result = await userManager.CreateAsync(user, cuvm.Password);

                if(result.Succeeded)
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
            if(await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            User user = await userManager.FindByIdAsync(id);

            if(user == null)
            {
                return NotFound();
            }

            EditUserViewModel euvm = new EditUserViewModel { Email = user.Email, Year = user.Year }; 
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel euvm)
        {
            if(await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            User user = await userManager.FindByIdAsync(euvm.Id);

            if(user != null)
            {
                user.Email = euvm.Email;
                user.UserName = euvm.Email;
                user.Year = euvm.Year;

                var result = await userManager.UpdateAsync(user);

                if(result.Succeeded)
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
            if(await userManager.GetUserAsync(HttpContext.User) == null)
                return new UnauthorizedResult();

            User user = await userManager.FindByIdAsync(id);
            
            if(user != null)
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
    }
}