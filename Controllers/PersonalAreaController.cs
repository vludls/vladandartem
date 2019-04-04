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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.RegularExpressions;
using vladandartem.Models;
using vladandartem.ClassHelpers;
using vladandartem.ViewModels.PersonalArea;
using Newtonsoft.Json;

namespace vladandartem.Controllers
{
    public class PersonalAreaController : Controller
    {
        private readonly ProductContext myDb;
        private readonly UserManager<User> userManager;
        //private readonly IHostingEnvironment HostEnv;

        public PersonalAreaController(ProductContext context, UserManager<User> userManager)
        {
            myDb = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Main()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PaidProducts()
        {
            User user = await userManager.GetUserAsync(HttpContext.User);

            if(user == null)
            {
                return NotFound();
            }

            var buff = userManager.Users.Where(u => u.Id == user.Id).Include(u => u.Order)
                .ThenInclude(u => u.CartProducts).ThenInclude(u => u.Product).FirstOrDefault();

            
            /*Cart cart = new Cart(HttpContext.Session, "cart");

            var cartProducts = from product in cart.Decode()
                    let buff = myDb.Products.Find(product.ProductId)
                    where buff != null
                    select buff;

            Cart cartPaid = new Cart(HttpContext.Session, "paid");

            var paidProducts = from product in cartPaid.Decode()
                    let buff = myDb.Products.Find(product.ProductId)
                    where buff != null
                    select new CartProduct { ProductId = product.ProductId, product = buff, ProductCount = product.ProductCount };
            
            MainViewModel mvm = new MainViewModel{ paidProducts = paidProducts };
            */
            return View(buff.Order);
        }
    }
}