using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperShop.Data.Entities;

namespace SuperShop.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email); // dou o email e dá uma string (Bypass)

        Task<IdentityResult> AddUserAsync(User user, string password);
    }
}
