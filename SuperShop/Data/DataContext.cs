using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    public class DataContext : IdentityDbContext<User> // Classe específica que fica responsável pela ligação á base de dados, injeto o meu user
    {

        public DbSet<Product> products { get; set; } // criar a tabela
                                                     //products -> propriedade que fica ligada á tabela Product através do DataContext
        public DataContext(DbContextOptions<DataContext> options) : base(options) // Injetar o DataContext da Entity Framework Core na minha
        {
        }
    }
}
