﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using vladandartem.Data.Models;

namespace vladandartem.Migrations
{
    [DbContext(typeof(ProductContext))]
    [Migration("20190506105845_Initial16")]
    partial class Initial16
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyStamp = "50f7014b-5668-4ec5-b4f0-e2389ce95550",
                            Name = "user",
                            NormalizedName = "user"
                        },
                        new
                        {
                            Id = 2,
                            ConcurrencyStamp = "84f9e41b-fdf6-455c-8243-c3058e98645c",
                            Name = "admin",
                            NormalizedName = "admin"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            RoleId = 2
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("vladandartem.Data.Models.Cart", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Cart");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            UserId = 1
                        });
                });

            modelBuilder.Entity("vladandartem.Data.Models.CartProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CartId");

                    b.Property<int>("Count");

                    b.Property<int?>("OrderId");

                    b.Property<int>("ProductId");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("CartProduct");
                });

            modelBuilder.Entity("vladandartem.Data.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("SectionId");

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Телефоны",
                            SectionId = 1
                        },
                        new
                        {
                            Id = 2,
                            Name = "Вентиляторы",
                            SectionId = 1
                        });
                });

            modelBuilder.Entity("vladandartem.Data.Models.Definition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DetailFieldId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("DetailFieldId");

                    b.ToTable("Definitions");
                });

            modelBuilder.Entity("vladandartem.Data.Models.DetailField", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("DetailFields");
                });

            modelBuilder.Entity("vladandartem.Data.Models.Order", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsPaid");

                    b.Property<int>("Number");

                    b.Property<DateTime>("OrderTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("SummaryPrice");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("vladandartem.Data.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId");

                    b.Property<int>("Count");

                    b.Property<string>("ImgPath")
                        .IsRequired();

                    b.Property<string>("Manufacturer")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Price");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CategoryId = 1,
                            Count = 1000,
                            ImgPath = "/images/Products/4180367_10.jpg",
                            Manufacturer = "Какойт",
                            Name = "Мейзу",
                            Price = 2500
                        },
                        new
                        {
                            Id = 2,
                            CategoryId = 1,
                            Count = 1000,
                            ImgPath = "/images/Products/4180367_10.jpg",
                            Manufacturer = "Какойт",
                            Name = "Нокия",
                            Price = 5000
                        },
                        new
                        {
                            Id = 3,
                            CategoryId = 2,
                            Count = 1000,
                            ImgPath = "/images/Products/4180367_10.jpg",
                            Manufacturer = "Какойт",
                            Name = "Вентилятор",
                            Price = 10000
                        });
                });

            modelBuilder.Entity("vladandartem.Data.Models.ProductDetailField", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DefinitionId");

                    b.Property<int>("DetailFieldId");

                    b.Property<int>("ProductId");

                    b.HasKey("Id");

                    b.HasIndex("DetailFieldId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductDetailFields");
                });

            modelBuilder.Entity("vladandartem.Data.Models.Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Sections");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Топ техника"
                        });
                });

            modelBuilder.Entity("vladandartem.Data.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<string>("Year");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "ee51d3ab-a39f-4d89-b646-55587eee7b8d",
                            Email = "Admin@yandex.ru",
                            EmailConfirmed = false,
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@YANDEX.RU",
                            NormalizedUserName = "ADMIN@YANDEX.RU",
                            PasswordHash = "AQAAAAEAACcQAAAAEBshpSUGttGjD8qSqQu6ey5fBBbTqDrPMVG4GFpjJPpXusbwdL7GUsLpQEd8KHzewg==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "STKLXSOQIOM7NR4EDU3TYNRP4ZHPRU7A",
                            TwoFactorEnabled = false,
                            UserName = "Admin@yandex.ru",
                            Year = "01/01/1984"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("vladandartem.Data.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("vladandartem.Data.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("vladandartem.Data.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("vladandartem.Data.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("vladandartem.Data.Models.Cart", b =>
                {
                    b.HasOne("vladandartem.Data.Models.User", "User")
                        .WithOne("Cart")
                        .HasForeignKey("vladandartem.Data.Models.Cart", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("vladandartem.Data.Models.CartProduct", b =>
                {
                    b.HasOne("vladandartem.Data.Models.Cart", "Cart")
                        .WithMany("CartProducts")
                        .HasForeignKey("CartId");

                    b.HasOne("vladandartem.Data.Models.Order", "Order")
                        .WithMany("CartProducts")
                        .HasForeignKey("OrderId");

                    b.HasOne("vladandartem.Data.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("vladandartem.Data.Models.Category", b =>
                {
                    b.HasOne("vladandartem.Data.Models.Section", "Section")
                        .WithMany()
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("vladandartem.Data.Models.Definition", b =>
                {
                    b.HasOne("vladandartem.Data.Models.DetailField", "DetailField")
                        .WithMany("Definitions")
                        .HasForeignKey("DetailFieldId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("vladandartem.Data.Models.Order", b =>
                {
                    b.HasOne("vladandartem.Data.Models.User", "User")
                        .WithMany("Order")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("vladandartem.Data.Models.Product", b =>
                {
                    b.HasOne("vladandartem.Data.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("vladandartem.Data.Models.ProductDetailField", b =>
                {
                    b.HasOne("vladandartem.Data.Models.DetailField", "DetailField")
                        .WithMany()
                        .HasForeignKey("DetailFieldId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("vladandartem.Data.Models.Product", "Product")
                        .WithMany("ProductDetailFields")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}