using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using vladandartem.Models.ViewModels.Account;
using vladandartem.Data.Models;
using vladandartem.Models;

namespace vladandartem.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly ProductContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(ProductContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [Route("Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return Content("123");
        }

        [Route("Register")]
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

                var result = await _userManager.CreateAsync(user, rvm.Password);

                if (result.Succeeded)
                {
                    Cart cart = new Cart { UserId = user.Id };

                    _context.Cart.Add(cart);

                    _context.SaveChanges();

                    await _userManager.AddToRoleAsync(user, "user");

                    await _signInManager.SignInAsync(user, false);

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

        [Route("Login")]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel{ ReturnUrl = returnUrl });
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel evm)
        {
            if (ModelState.IsValid)
            {
                var result = 
                    await _signInManager.PasswordSignInAsync(evm.Email, evm.Password, evm.RememberMe, false);
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
        
        [Route("LogOff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            
            return RedirectToAction("Index", "Home");
        }
    }
}