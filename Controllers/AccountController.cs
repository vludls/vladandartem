using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using vladandartem.ViewModels.Account;
using vladandartem.Models;

namespace vladandartem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel rvm)
        {
            if(ModelState.IsValid)
            {
                User user = new User { Email = rvm.Email, UserName = rvm.Email, Year = rvm.Year };

                var result = await userManager.CreateAsync(user, rvm.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);

                    return Redirect("~/PersonalArea/Main");
                }
                else
                {
                    string err = "";
                    foreach (var error in result.Errors)
                    {
                        err += $"{error.Description}\n";
                        return Content(err);
                    }
                }

                return Content("123");
            }

            return View(rvm);
        }

        [HttpGet]
        public IActionResult Authentication()
        {
            return View();
        }
    }
}