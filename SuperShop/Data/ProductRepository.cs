using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using System.Linq;

namespace SuperShop.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.products.Include(p => p.User); // como se fosse um join do SQL
        }
    }
}