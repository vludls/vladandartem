using System;
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
            CreateMap<CreateUserViewModel, User>();
            CreateMap<Product, AnalyticsLoadProductsOfChoosedCategoryViewModel>();
        }
    }
}
