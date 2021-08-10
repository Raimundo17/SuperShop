using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperShop.Data.Entities;
using SuperShop.Helpers;

namespace SuperShop.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        //private readonly UserManager<User> _userManager;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            //_userManager = userManager;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync(); // ver se a base de dados está criada

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");

            var user = await _userHelper.GetUserByEmailAsync("daniel.raimundo.21229@formandos.cinel.pt"); // ver se ja existe utilizador, o primeiro a ser criado é o admin pela propria aplicacao
            if(user == null) // se nao existir
            {
                user = new User
                {
                    FirstName = "Daniel",
                    LastName = "Raimundo",
                    Email = "daniel.raimundo.21229@formandos.cinel.pt",
                    UserName = "daniel.raimundo.21229@formandos.cinel.pt",
                    PhoneNumber = "123456789"
                };

                var result = await _userHelper.AddUserAsync(user, "123456"); 
                                                            // os dois paramentros a passar é o user e a password á parte do objeto para poder ser
                                                            // encriptada
                if(result != IdentityResult.Success) // se houve algum problema a criar
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
            if(!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            if (!_context.products.Any()) // se nao existirem produtos
            {
                AddProduct("iPhone X", user);
                AddProduct("Magic Mouse", user);
                AddProduct("iWatch Series 4", user);
                AddProduct("iPad mini", user);
                await _context.SaveChangesAsync(); // grava na base de dados
            }
        }

        private void AddProduct(string name, User user)
        {
            _context.products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000),
                IsAvailable = true,
                Stock = _random.Next(1000),
                User = user
            });
        }
    }
}
