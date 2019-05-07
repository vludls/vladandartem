﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using vladandartem.Data.Models;
using vladandartem.Models;
using vladandartem.Models.ViewModels.AdminMenu;
using vladandartem.Models.Request.AdminMenu;

namespace vladandartem.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserAvailableInfoReturn>();
            CreateMap<UserSaveModel, User>();
            CreateMap<Product, Product>();
            CreateMap<ProductDetailField, ProductDetailField>().ForMember(p => p.Product, opt => opt.UseDestinationValue());
            CreateMap<Definition, Definition>().ForMember(p => p.DetailField, opt => opt.UseDestinationValue());
            CreateMap<Order, Order>().ForMember(p => p.User, opt => opt.UseDestinationValue());
            CreateMap<CartProduct, CartProduct>().ForMember(p => p.Order, opt => opt.UseDestinationValue())
                .ForMember(p => p.Cart, opt => opt.UseDestinationValue());
            CreateMap<CreateUserViewModel, User>();
            CreateMap<Product, AnalyticsLoadProductsOfChoosedCategoryViewModel>();
            CreateMap<ProductDetailField, ProductDetailFieldAvailableInfoReturn>();
        }
    }
}