using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Data.Entities;

namespace SuperShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.products.ToListAsync()); // trás todos os produtos
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create() // Abrir a view do create (aquela janela que aparece assim que carregamos no botao do create new)
        {
            return View();
        }

        // Este Post corresponde ao botão create que aparece em baixo quando acabamos de preencher a informacao do novo produto
        //Recebe o modelo e envia para a base de dados
        // POST: Products/Create 
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product) //Aqui já recebe o objeto
        {                               
            if (ModelState.IsValid)
            {
                _context.Add(product); // recebe o produto
                await _context.SaveChangesAsync(); // guarda na base de dados de forma assíncrona
                return RedirectToAction(nameof(Index)); // redireciona para a action index (mostra a lista dos produtos)
            }
            return View(product); // se o produto não passar nas validações mostra a view e deixa ficar lá o produto,
                                    // para o utilizador não ter que preencher tudo de novo
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id) 
        {
            if (id == null) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
            {
                return NotFound();
            }

            var product = await _context.products.FindAsync(id); // coloca o id em memória e verifica caso o id tenha sido eliminado entretanto
            if (product == null)
            {
                return NotFound();
            }
            return View(product); // retorna a view e manda o produto lá para dentro
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product); // faz o update do produto
                    await _context.SaveChangesAsync(); // grava na base de dados
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) // verifica se o id existe devido a alguem entretanto ter apagado este produto
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5 // Só mostra o que for para apagar. Não apaga
        public async Task<IActionResult> Delete(int? id) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")] // quando houver um action chamada "Delete" mas que seja com um Post faz o DeleteConfirmed
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // o id é obrigatório
        {
            var product = await _context.products.FindAsync(id); // o id é verficado para ver se ainda existe
            _context.products.Remove(product); //remover em memória
            await _context.SaveChangesAsync(); // guarda as alterações na base de dados
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.products.Any(e => e.Id == id);
        }
    }
}
