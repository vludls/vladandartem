using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vladandartem.Models;
using Microsoft.AspNetCore.Identity;

namespace vladandartem.Initializer
{
    public static class Db
    {
        public static void Initialize(ProductContext context)
        {
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
        }
    }
}
