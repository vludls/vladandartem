using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Identity.Core;
using vladandartem.ViewModels.Account;
using vladandartem.Models;

namespace vladandartem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ProductContext context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly SignInManager<User> signInManager;
        public AccountController(ProductContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<User> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
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
                //context.Cart.Add(new Cart());
                User user = new User {
                    Email = rvm.Email,
                    UserName = rvm.Email, 
                    Year = rvm.Year
                };

                var result = await userManager.CreateAsync(user, rvm.Password);

                if (result.Succeeded)
                {
                    Cart cart = new Cart { UserId = user.Id };

                    context.Cart.Add(cart);

                    context.SaveChanges();

                    await userManager.AddToRoleAsync(user, "user");

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
            }

            return View(rvm);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel{ ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel evm)
        {
            if (ModelState.IsValid)
            {
                var result = 
                    await signInManager.PasswordSignInAsync(evm.Email, evm.Password, evm.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(evm.ReturnUrl) && Url.IsLocalUrl(evm.ReturnUrl))
                    {
                        return Redirect(evm.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(evm);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            // удаляем аутентификационные куки
            await signInManager.SignOutAsync();
            
            return RedirectToAction("Index", "Home");
        }
    }
}