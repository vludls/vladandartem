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

namespace vladandartem.Controllers
{
    public class AdminMenuController : Controller
    {
        private UserManager<User> userManager;
        private ProductContext myDb;

        public AdminMenuController(ProductContext context, UserManager<User> userManager)
        {
            this.userManager = userManager;
            myDb = context;
        }
        public IActionResult Main()
        {
            return View(myDb.Categories.ToList());
        }

        [HttpPost]
        public IActionResult AddCategory(string CategoryName)
        {
            myDb.Categories.Add(new Category{ Name = CategoryName });

            myDb.SaveChanges();
            
            return Redirect("~/AdminMenu/Main");
        }
        [HttpPost]
        public IActionResult DeleteCategory(int CategoryId)
        {
            Category SomeCategory = myDb.Categories.Find(CategoryId);

            myDb.Categories.Remove(SomeCategory);

            myDb.SaveChanges();

            return Redirect("~/AdminMenu/Main");
        }

        [HttpGet]
        public IActionResult Users() => View(userManager.Users.ToList());

        [HttpGet]
        public IActionResult CreateUser() => View();

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel cuvm)
        {
            User user = new User { Email = cuvm.Email, UserName = cuvm.Email, Year = cuvm.Year,  };
            
            if(ModelState.IsValid)
            {
                var result = await userManager.CreateAsync(user, cuvm.Password);

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
            return View(cuvm);
        }
        
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            User user = await userManager.FindByIdAsync(id);

            if(user == null)
            {
                return NotFound();
            }

            EditUserViewModel euvm = new EditUserViewModel { Email = user.Email, Year = user.Year }; 

            return View(euvm);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel euvm)
        {
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
            User user = await userManager.FindByIdAsync(id);

            if(user != null)
            {
                await userManager.DeleteAsync(user);
            }
            return RedirectToAction("Users");
        }
    }
}