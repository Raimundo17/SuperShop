using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserHelper _userHelper;

        public ProductsController(
            IProductRepository productRepository, IUserHelper userHelper)
        {
            _productRepository = productRepository;
            _userHelper = userHelper;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productRepository.GetAll().OrderBy(p=> p.Name)); // trás todos os produtos, ordenados alfabeticamente pelo nome
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id) // pode aceitar null
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value); // tem que ser id.value para que se for null não "rebentar"
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
        public async Task<IActionResult> Create(ProductViewModel model) //Aqui já recebe o objeto
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty; // caminho da imagem

                if(model.ImageFile != null && model.ImageFile.Length > 0) // verificar se tem imagem
                {
                    var guid = Guid.NewGuid().ToString(); // alterar o nome do ficheiro inserido
                    var file = $"{guid}.jpg";

                    path = Path.Combine(    // para montar o caminho
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\products",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream); //gravar no servidor
                    }

                    path = $"~/images/products/{file}"; // defino o caminho para depois gravar na base de dados
                }                                                           // no campo imageUrl

                var product = this.ToProduct(model,path); // coverte de product para view model

                //TODO : Modificar para o user que tiver logado
                product.User = await _userHelper.GetUserByEmailAsync("daniel.raimundo.21229@formandos.cinel.pt");
                await _productRepository.CreateAsync(product); // recebe o produto
                return RedirectToAction(nameof(Index)); // redireciona para a action index (mostra a lista dos produtos)
            }
            return View(model); // se o produto não passar nas validações mostra a view e deixa ficar lá o produto,
                                  // para o utilizador não ter que preencher tudo de novo
        }

        private Product ToProduct(ProductViewModel model, string path) // coverte de product para view model
        {
            return new Product
            {
                Id = model.Id,
                ImageUrl = path,
                IsAvailable = model.IsAvailable,
                LastPurchase = model.LastPurchase,
                LastSale = model.LastSale,
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock,
                User = model.User
            };
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value); // coloca o id em memória e verifica caso o id tenha sido eliminado entretanto
            if (product == null)                            // tem que ser id.value para que se for null não "rebentar"
            {
                return NotFound();
            }

            var model = this.ToProductViewModel(product); //vai á base de dados e converte de product para um product view model

            return View(model); // retorna a view e manda o produto lá para dentro
        }

        private ProductViewModel ToProductViewModel(Product product) // converte de product para um product view model
        {
            return new ProductViewModel
            {
                Id = product.Id,
                IsAvailable = product.IsAvailable,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if(model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString(); // alterar o nome do ficheiro inserido
                        var file = $"{guid}.jpg";

                        path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\products",
                            file);

                        using(var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/images/products/{file}";
                    }

                    var product = this.ToProduct(model, path);

                    //TODO : Modificar para o user que tiver logado
                    product.User = await _userHelper.GetUserByEmailAsync("daniel.raimundo.21229@formandos.cinel.pt"); 
                    await _productRepository.UpdateAsync(product); // faz o update do produto
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await _productRepository.ExistAsync(model.Id)) // verifica se o id existe devido a alguem entretanto ter apagado este produto
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
            return View(model);
        }

        // GET: Products/Delete/5 // Só mostra o que for para apagar. Não apaga
        public async Task <IActionResult> Delete(int? id) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
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
            var product = await _productRepository.GetByIdAsync(id); // o id é verficado para ver se ainda existe
            await _productRepository.DeleteAsync(product); //remover em memória
            return RedirectToAction(nameof(Index));
        }
    }
}
