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
using vladandartem.ClassHelpers;
using Newtonsoft.Json;

namespace vladandartem.Controllers
{
    public class PersonalAreaController : Controller
    {
        private ProductContext myDb;
        //private readonly IHostingEnvironment HostEnv;

        public PersonalAreaController(ProductContext context)
        {
            myDb = context;
        }

        [HttpGet]
        public IActionResult Main()
        {
            Cart cart = new Cart(HttpContext.Session, "cart");

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
            
            return View(mvm);
        }
    }
}