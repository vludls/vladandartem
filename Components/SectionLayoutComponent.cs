using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using vladandartem.Models;
using System.Threading.Tasks;
using vladandartem.Data.Models;

namespace vladandartem.Components
{
    public class SectionLayout : ViewComponent
    {
        private readonly ProductContext context;

        public SectionLayout(ProductContext context)
        {
            this.context = context;
        }

        public IViewComponentResult Invoke()
        {
            return View("SectionComponent", context.Sections.ToList());
        }
    }
}
