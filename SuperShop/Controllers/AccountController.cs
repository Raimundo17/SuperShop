using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;

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

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if(ModelState.IsValid) // se os campos estao devidamente preenchidos
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username); // verificar se este user já existe ou não
                if(user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if(result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn´t be created");
                        return View(model);
                    }

                    var loginViewModel = new LoginViewModel // faz o login pelo utilizador
                    {
                        Password = model.Password,
                        RememberMe = false,
                        UserName = model.Username
                    };

                    var result2 = await _userHelper.LoginAsync(loginViewModel);
                    if(result2.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "The user couldn´t be logged");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserViewModel();
            if(user !=null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            { 
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                var response = await _userHelper.UpdateUserAsync(user);
                if (response.Succeeded)
                {
                    ViewBag.UserMessage = "User updated!";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                }
            }
        }
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View(); // só se passa a view para as caixas estarem vazias, nao queremos preencher as caixas com as passwords
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user,model.OldPassword,model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty,"User not found." );
                }
            }
            return View(model);
        }

        public IActionResult NotAuthorized()
        {
            return View(); // em vez de passar pelo middleware todas as páginas de nao autorizado passaram por aqui (em vez de mandar para o login)
        }
    }
}
