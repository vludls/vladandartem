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
using Microsoft.AspNetCore.Authorization;

namespace vladandartem.Controllers
{
    [Authorize]
    public class PersonalAreaController : Controller
    {
        private readonly ProductContext myDb;
        private readonly UserManager<User> userManager;

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
        [HttpPost]
        public IActionResult PayOrder(int id)
        {
            Order order = myDb.Orders.Find(id);

            order.IsPaid = true;

            order.OrderTime = DateTime.UtcNow;

            myDb.SaveChanges();

            return RedirectToAction("PaidProducts");
        }
        [HttpGet]
        public async Task<IActionResult> PaidProducts()
        {
            User user = await userManager.GetUserAsync(HttpContext.User);

            var buff = userManager.Users.Include(u => u.Order)
            .ThenInclude(u => u.CartProducts)
            .ThenInclude(u => u.Product)
            .FirstOrDefault(u => u.Id == user.Id);

            if (buff == null)
                return NotFound();

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
            return View(buff.Order.OrderByDescending(n => n.Number).ToList());
        }
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            //myDb.Categories.Include
            User user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return NotFound();
            }

            var buff = userManager.Users.Where(u => u.Id == user.Id).Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts)
                .ThenInclude(g => g.Product)
                .ThenInclude(u => u.Category).FirstOrDefault();

            if (buff == null)
            {
                return NotFound();
            }
            //var orders = myDb.Orders.Where(order => order.UserId == user.Id);

            //List<CartProduct> cartProduct =
            //JsonConvert.DeserializeObject<List<CartProduct>>(user.CartJSON);
            /*
            Cart cart = new Cart(HttpContext.Session, "cart");

            var products = from element in cart.Decode()
                        let buff = myDb.Products.Find(element.ProductId)
                        where buff != null
                        select new CartProduct {
                            ProductId = element.ProductId,
                            product = buff,
                            ProductCount = element.ProductCount
                        };
            */
            return View(buff.Cart.CartProducts);
        }
        [HttpPost]
        public async Task<IActionResult> CartOrder()
        {
            User user = await userManager.GetUserAsync(HttpContext.User);

            var buff = userManager.Users.Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts).ThenInclude(u => u.Product).FirstOrDefault(u => u.Id == user.Id);

            foreach (var cartProduct in buff.Cart.CartProducts)
            {
                if(cartProduct.Count > cartProduct.Product.Count)
                    ModelState.AddModelError(string.Empty, $"{cartProduct.Product.Name}: на складе есть только лишь {cartProduct.Product.Count} шт. товара, в заказе указано {cartProduct.Count}");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var order = new Order
            {
                UserId = user.Id,
                Number = (myDb.Orders.Any() ? myDb.Orders.OrderBy(n => n.Number).Last().Number + 1 : 1),
                SummaryPrice = buff.Cart.CartProducts.Sum(n => n.Product.Price * n.Count)
            };

            myDb.Orders.Add(order);

            myDb.SaveChanges();

            foreach (var cp in buff.Cart.CartProducts)
            {
                cp.Product.Count -= cp.Count;
                cp.CartId = null;
                cp.OrderId = order.Id;
            }

            //buff.Cart.CartProducts.Remove(buff.Cart.CartProducts.Find(n => n.ProductId == id));

            await userManager.UpdateAsync(buff);

            return RedirectToAction("PaidProducts", "PersonalArea");
        }
    }
}