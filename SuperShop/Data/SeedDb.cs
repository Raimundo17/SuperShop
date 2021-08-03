using System;
using System.Linq;
using System.Threading.Tasks;
using SuperShop.Data.Entities;


namespace SuperShop.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync(); // ver se a base de dados está criada

            if (!_context.products.Any()) // se nao existirem produtos
            {
                AddProduct("iPhone X");
                AddProduct("Magic Mouse");
                AddProduct("iWatch Series 4");
                AddProduct("iPad mini");
                await _context.SaveChangesAsync(); // grava na base de dados
            }
        }

        private void AddProduct(string name)
        {
            _context.products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000),
                IsAvailable = true,
                Stock = _random.Next(1000)
            });
        }
    }
}
