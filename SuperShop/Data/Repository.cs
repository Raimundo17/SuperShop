using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    public class Repository : IRepository
    {
        private readonly DataContext _context;

        public Repository(DataContext context)
        {
            _context = context;
        }

        //READ
        public IEnumerable<Product> GetProducts() // Devolve todos os produtos
        {
            return _context.products.OrderBy(p => p.Name);
        }

        //READ/ID
        public Product GetProduct(int id) // Devolve o produto pelo Id
        {
            return _context.products.Find(id);
        }

        //CREATE
        public void AddProduct(Product product)
        {
            _context.products.Add(product); // adiciona o produto em memória
                                            //Add -> Entity FrameWork
        }

        //EDIT
        public void UpdateProduct(Product product) // EDIT
        {
            _context.products.Update(product); // faz update do produto que entrou como parâmetro
                                               //Update -> Entity FrameWork
        }

        //DELETE
        public void RemoveProduct(Product product) // EDIT
        {
            _context.products.Remove(product); // faz update do produto que entrou como parâmetro
                                               //Remove -> Entity FrameWork
        }

        public async Task<bool> SaveAllAsync() //gravar para a base de dados
        {
            return await _context.SaveChangesAsync() > 0; // grava pelo menos uma coisa, permite que o programe nao "bloqueie"
        }

        public bool ProductExists(int id)
        {
            return _context.products.Any(p => p.Id == id); // existe algum produto com este id
        }
    }
}
