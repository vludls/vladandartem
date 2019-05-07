using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;
using System.Linq;

namespace vladandartem.Data.Models
{
    public class ProductContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public IPasswordHasher<User> _passwordHasher;

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartProduct> CartProduct { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Definition> Definitions { get; set; }
        public DbSet<DetailField> DetailFields { get; set; }
        public DbSet<ProductDetailField> ProductDetailFields { get; set; }

        public ProductContext(DbContextOptions<ProductContext> options, IPasswordHasher<User> passwordHasher) : base(options)
        {
            _passwordHasher = passwordHasher;
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region SeedRoles

            modelBuilder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int>
                {
                    Id = 1,
                    Name = "user",
                    NormalizedName = "user"
                },
                new IdentityRole<int>
                {
                    Id = 2,
                    Name = "admin",
                    NormalizedName = "admin"
                }
            );

            #endregion

            #region SeedUsers

            User user = new User
            {
                Id = 1,
                Email = "Admin@yandex.ru",
                NormalizedEmail = "ADMIN@YANDEX.RU",
                UserName = "Admin@yandex.ru",
                NormalizedUserName = "ADMIN@YANDEX.RU",
                Year = "01/01/1984",
                SecurityStamp = "STKLXSOQIOM7NR4EDU3TYNRP4ZHPRU7A"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, "123456aA!");

            modelBuilder.Entity<User>().HasData(user);

            modelBuilder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int> { UserId = user.Id, RoleId = 2 });

            modelBuilder.Entity<Cart>().HasData(new Cart { Id = 1, UserId = user.Id });

            #endregion

            #region SeedSections

            modelBuilder.Entity<Section>().HasData(new Section { Id = 1, Name = "Топ техника" });

            #endregion

            #region SeedCategories

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Телефоны", SectionId = 1 },
                new Category { Id = 2, Name = "Вентиляторы", SectionId = 1 }
            );

            #endregion

            #region SeedProducts

            List<Product> pr = new List<Product>();

            pr.AddRange(new Product[] {
                new Product
                {
                    Id = 1,
                    Name = "Мейзу",
                    Price = 2500,
                    ImgPath = "/images/Products/4180367_10.jpg",
                    CategoryId = 1,
                    Manufacturer = "Какойт",
                    Count = 1000
                },
                new Product
                {
                    Id = 2,
                    Name = "Нокия",
                    Price = 5000,
                    ImgPath = "/images/Products/4180367_10.jpg",
                    CategoryId = 1,
                    Manufacturer = "Какойт",
                    Count = 1000
                },
                new Product
                {
                    Id = 3,
                    Name = "Вентилятор",
                    Price = 10000,
                    ImgPath = "/images/Products/4180367_10.jpg",
                    CategoryId = 2,
                    Manufacturer = "Какойт",
                    Count = 1000
                }
            });

            modelBuilder.Entity<Product>().HasData(pr);

            #endregion

            #region SeedOrders
            DateTime dateBuff = new DateTime(2019, 4, 15);
            DateTime dateEnd = new DateTime(2021, 4, 15);

            int orderCounter = 0;
            int cartProductCounter = 0;

            while (DateTime.Compare(dateBuff, dateEnd) < 0)
            {
                //var buff = this.Users.Include(u => u.Cart)
                    //.ThenInclude(u => u.CartProducts).ThenInclude(u => u.Product)
                    //.FirstOrDefault(u => u.Id == 1);

                List<CartProduct> cps = new List<CartProduct>();

                Order order = new Order
                {
                    Id = ++orderCounter,
                    UserId = 1,
                    Number = orderCounter,
                    SummaryPrice = pr[0].Price + pr[1].Price + pr[2].Price, // Здесь ошибка
                    IsPaid = true,
                    OrderTime = dateBuff
                };

                modelBuilder.Entity<Order>().HasData(order);

                cps.AddRange(new CartProduct[] {
                    new CartProduct { Id = ++cartProductCounter, ProductId = 1, Count = 4, OrderId = orderCounter },
                    new CartProduct { Id = ++cartProductCounter, ProductId = 2, Count = 5, OrderId = orderCounter },
                    new CartProduct { Id = ++cartProductCounter, ProductId = 3, Count = 6, OrderId = orderCounter }
                }
                );

                modelBuilder.Entity<CartProduct>().HasData(cps);

                //modelBuilder.
                //this.Users.Update(buff);

                //modelBuilder.Entity<Order>.

                /*foreach (var cp in buff.Cart.CartProducts)
                {
                    cp.Product.Count--;
                    cp.CartId = null;
                    cp.OrderId = order.Id;
                }*/

                dateBuff = dateBuff.AddDays(1);
            }
            #endregion
        }
    }
}