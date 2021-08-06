using Microsoft.AspNetCore.Mvc;
using SuperShop.Helpers;
using SuperShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated) // se o utilizador quando fizer login já estiver autenticado
            {
                return RedirectToAction("Index", "Home"); // primeiro a action depois o controlador home
            }

            return View(); // se acontecer alguma coisa de errado fica na mesma view
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid) // se estiver tudo preenchido(email, password requerido pelos data notations)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                { // se na query(url) aparecer um ReturnUrl ele reencaminha o utilizador depois de fazer login para a página onde ia inicialmente
                    if (this.Request.Query.Keys.Contains("ReturnUrl")) 
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First()); // o primeiro returnUrl que encontrar
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }
            this.ModelState.AddModelError(string.Empty, "Failed to login"); 
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
