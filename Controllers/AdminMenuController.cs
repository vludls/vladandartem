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
    public class AdminMenuController : Controller
    {
        private ProductContext myDb;
        //private readonly IHostingEnvironment HostEnv;

        public AdminMenuController(ProductContext context)
        {
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
    }
}