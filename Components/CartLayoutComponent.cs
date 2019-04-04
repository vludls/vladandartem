using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using vladandartem.Models;

namespace vladandartem.Components
{
    public class CartLayout : ViewComponent
    {
        private readonly ProductContext context;
        private readonly UserManager<User> userManager;

        public CartLayout(ProductContext context, UserManager<User> userManager)
        {
            this.context = context;

            this.userManager = userManager;
        }

        public async Task<string> InvokeAsync()
        {
            User user = await userManager.GetUserAsync(HttpContext.User);

            return Convert.ToString(userManager.Users.Where(u => u.Id == user.Id)
                .Include(u => u.Cart)
                .ThenInclude(u => u.CartProducts)
                .FirstOrDefault()
                .Cart.CartProducts.Count());
        }
    }
}