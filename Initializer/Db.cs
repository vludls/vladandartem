using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vladandartem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using vladandartem.ClassHelpers;
using Microsoft.EntityFrameworkCore;

namespace vladandartem.Initializer
{
    public static class Db
    {
        public async static void Initialize(ProductContext context, IPasswordHasher<User> passwordHasher)
        {
            // Инициализация ролей
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new IdentityRole<int>
                    {
                        Name = "user",
                        NormalizedName = "user"
                    },
                    new IdentityRole<int>
                    {
                        Name = "admin",
                        NormalizedName = "admin"
                    }
                );
                context.SaveChanges();
            }

            // Инициализация пользователя
            if (!context.Users.Any())
            {
                User user = new User
                {
                    Email = "Admin@yandex.ru",
                    NormalizedEmail = "ADMIN@YANDEX.RU",
                    UserName = "Admin@yandex.ru",
                    NormalizedUserName = "ADMIN@YANDEX.RU",
                    Year = "01/01/1984",
                    SecurityStamp = "STKLXSOQIOM7NR4EDU3TYNRP4ZHPRU7A"
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "123456aA!");

                context.Users.Add(user);

                context.UserRoles.Add(new IdentityUserRole<int> { UserId = user.Id, RoleId = 2 });

                //var result = await userManager.CreateAsync(user, "123456aA!");

                Cart cart = new Cart { UserId = user.Id };

                context.Cart.Add(cart);

                context.SaveChanges();

               // await userManager.AddToRoleAsync(user, "admin");
            }

            if (!context.Sections.Any())
            {
                context.Sections.Add(new Section { Name = "Топ техника" });

                context.SaveChanges();
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Телефоны", SectionId = 1 },
                    new Category { Name = "Вентиляторы", SectionId = 1 }
                );

                context.SaveChanges();
            }
            // Инициализация продуктов
            if (!context.Products.Any())
            {
                List<Product> products = new List<Product>();

                products.Add(new Product
                {
                    Name = "Мейзу",
                    Price = 2500,
                    ImgPath = "/images/Products/4180367_10.jpg",
                    CategoryId = 1,
                    Manufacturer = "Какойт",
                    Count = 1000
                });

                products.Add(new Product
                {
                    Name = "Нокия",
                    Price = 5000,
                    ImgPath = "/images/Products/4180367_10.jpg",
                    CategoryId = 1,
                    Manufacturer = "Какойт",
                    Count = 1000
                });

                products.Add(new Product
                {
                    Name = "Вентилятор",
                    Price = 10000,
                    ImgPath = "/images/Products/4180367_10.jpg",
                    CategoryId = 2,
                    Manufacturer = "Какойт",
                    Count = 1000
                });

                context.Products.AddRange(products);

                context.SaveChanges();
            }

            // Имитация заказов
            if (!context.Orders.Any())
            {
                DateTime dateBuff = new DateTime(2019, 4, 15);
                DateTime dateEnd = new DateTime(2021, 4, 15);

                while (DateTime.Compare(dateBuff, dateEnd) < 0)
                {
                    var buff = context.Users.Include(u => u.Cart)
                        .ThenInclude(u => u.CartProducts).ThenInclude(u => u.Product)
                        .FirstOrDefault(u => u.Id == 1);

                    context.CartProduct.AddRange(
                        new CartProduct { ProductId = 1, Count = 4, CartId = buff.Cart.Id },
                        new CartProduct { ProductId = 2, Count = 5, CartId = buff.Cart.Id },
                        new CartProduct { ProductId = 3, Count = 6, CartId = buff.Cart.Id }
                    );
                    
                    context.Users.Update(buff);

                    Order order = new Order
                    {
                        UserId = buff.Id,
                        Number = (context.Orders.Any() ? context.Orders.OrderBy(n => n.Number).Last().Number + 1 : 1),
                        SummaryPrice = buff.Cart.CartProducts.Sum(n => n.Product.Price * n.Count)
                    };

                    context.Orders.Add(order);

                    foreach (var cp in buff.Cart.CartProducts)
                    {
                        cp.Product.Count--;
                        cp.CartId = null;
                        cp.OrderId = order.Id;
                    }

                    order.IsPaid = true;

                    order.OrderTime = dateBuff;

                    context.SaveChanges();

                    dateBuff = dateBuff.AddDays(1);
                }
            }
        }
    }
}
