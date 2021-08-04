using Microsoft.AspNetCore.Identity;

namespace SuperShop.Data.Entities
{
    public class User : IdentityUser
    {
        // Além das propiedades por defeito, acrescento as minhas

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}
