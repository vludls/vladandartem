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
            List<Product> CartProducts = new List<Product>();

            Cart cart = new Cart(HttpContext.Session, "cart");

            foreach(var product in cart.Decode())
            {
                Product productBuff = myDb.Products.Find(product.ProductId);

                if(productBuff != null)
                    CartProducts.Add(productBuff);
            }

            List<Product> PaidProducts = new List<Product>();

            Cart cartPaid = new Cart(HttpContext.Session, "paid");

            foreach(var product in cartPaid.Decode())
            {
                Product productBuff = myDb.Products.Find(product.ProductId);

                if(productBuff != null)
                    PaidProducts.Add(productBuff);
            }

            ViewBag.CartProducts = CartProducts;
            ViewBag.PaidProducts = PaidProducts;

            return View();
        }
    }
}