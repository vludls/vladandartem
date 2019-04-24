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
        private readonly ProductContext _context;
        private readonly UserManager<User> _userManager;

        public PersonalAreaController(ProductContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public ViewResult Main()
        {
            return View();
        }
        [HttpPost]
        public RedirectToActionResult PayOrder(int id)
        {
            Order order = _context.Orders.Find(id);

            order.IsPaid = true;

            order.OrderTime = DateTime.UtcNow;

            _context.SaveChanges();

            return RedirectToAction("PaidProducts");
        }
        [HttpGet]
        public ViewResult PaidProducts()
        {
            return View();
        }
        [HttpPost]
        public async Task<ContentResult> GetPaidProducts()
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            user = _userManager.Users.Include(u => u.Order)
                .ThenInclude(u => u.CartProducts)
                .ThenInclude(u => u.Product)
                .FirstOrDefault(u => u.Id == user.Id);

            return Content(JsonConvert.SerializeObject(user.Order.OrderByDescending(n => n.Number)).ToList());
        }
        [HttpGet]
        public ViewResult Cart()
        {
            return View();
        }
        [HttpPost]
        public async Task<ContentResult> CartGetCartProducts()
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            user = _userManager.Users.Where(u => u.Id == user.Id).Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts)
                .ThenInclude(g => g.Product)
                .ThenInclude(u => u.Category)
                .FirstOrDefault();
            
            return Content(JsonConvert.SerializeObject(user.Cart.CartProducts));
        }

        [HttpPost]
        public async Task<IActionResult> CartOrder()
        {
            User user = await _userManager.GetUserAsync(HttpContext.User);

            user = _userManager.Users.Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts)
                .ThenInclude(u => u.Product)
                .FirstOrDefault(u => u.Id == user.Id);

            foreach (var cartProduct in user.Cart.CartProducts)
            {
                if(cartProduct.Count > cartProduct.Product.Count)
                    ModelState.AddModelError(string.Empty, $"{cartProduct.Product.Name}: �� ������ ���� ������ ���� {cartProduct.Product.Count} ��. ������, � ������ ������� {cartProduct.Count}");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var order = new Order
            {
                UserId = user.Id,
                Number = (_context.Orders.Any() ? _context.Orders.OrderBy(n => n.Number).Last().Number + 1 : 1),
                SummaryPrice = user.Cart.CartProducts.Sum(n => n.Product.Price * n.Count)
            };

            _context.Orders.Add(order);

            _context.SaveChanges();

            foreach (var cp in user.Cart.CartProducts)
            {
                cp.Product.Count -= cp.Count;
                cp.CartId = null;
                cp.OrderId = order.Id;
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction("PaidProducts", "PersonalArea");
        }

        [HttpPost]
        public IActionResult RejectOrder(int orderId)
        {
            Order order = _context.Orders.Include(o => o.CartProducts)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null || order.IsPaid)
                return new EmptyResult();

            foreach (var cartProduct in order.CartProducts)
            {
                cartProduct.Product.Count += cartProduct.Count;
            }

            _context.Orders.Remove(order);

            return Content(JsonConvert.SerializeObject(new { OrderId = order.Id }));
        }
    }
}