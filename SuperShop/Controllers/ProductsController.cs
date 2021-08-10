using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Helpers;
using SuperShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public ProductsController(
            IProductRepository productRepository, IUserHelper userHelper, IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _productRepository = productRepository;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productRepository.GetAll().OrderBy(p => p.Name)); // trás todos os produtos, ordenados alfabeticamente pelo nome
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id) // pode aceitar null
        {
            if (id == null)
            {
                //return NotFound();
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view
                                        // genérico dá para produtos, clientes, fornecedores, etc
            }

            var product = await _productRepository.GetByIdAsync(id.Value); // tem que ser id.value para que se for null não "rebentar"
            if (product == null)
            {
                //return NotFound();
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles ="Admin")]
        //[Authorize(Roles = "Admin,Customer")]
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

                if (model.ImageFile != null && model.ImageFile.Length > 0) // verificar se tem imagem
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "products"); // guarda o ficheiro na pasta products
                }

                // coverte de product para view model
                var product = _converterHelper.ToProduct(model, path, true); // é true porque é novo (create)

                
                product.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); // dá o utilizador que estiver "logado"
                await _productRepository.CreateAsync(product); // recebe o produto
                return RedirectToAction(nameof(Index)); // redireciona para a action index (mostra a lista dos produtos)
            }
            return View(model); // se o produto não passar nas validações mostra a view e deixa ficar lá o produto,
                                // para o utilizador não ter que preencher tudo de novo
        }

        // OLD
        //private Product ToProduct(ProductViewModel model, string path) // coverte de product para view model
        //{
        //    return new Product
        //    {
        //        Id = model.Id,
        //        ImageUrl = path,
        //        IsAvailable = model.IsAvailable,
        //        LastPurchase = model.LastPurchase,
        //        LastSale = model.LastSale,
        //        Name = model.Name,
        //        Price = model.Price,
        //        Stock = model.Stock,
        //        User = model.User
        //    };
        //}

        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
            {
                // return NotFound();
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view
                                                                  // genérico dá para produtos, clientes, fornecedores, etc
            }

            var product = await _productRepository.GetByIdAsync(id.Value); // coloca o id em memória e verifica caso o id tenha sido eliminado entretanto
            if (product == null)                            // tem que ser id.value para que se for null não "rebentar"
            {
                //return NotFound();
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view
                                                                  // genérico dá para produtos, clientes, fornecedores, etc
            }

            var model = _converterHelper.ToProductViewModel(product); // converte de product para um product view model
            return View(model); // retorna a view e manda o produto lá para dentro
        }

        // OLD
        //private ProductViewModel ToProductViewModel(Product product) // converte de product para um product view model
        //{
        //    return new ProductViewModel
        //    {
        //        Id = product.Id,
        //        IsAvailable = product.IsAvailable,
        //        LastPurchase = product.LastPurchase,
        //        LastSale = product.LastSale,
        //        ImageUrl = product.ImageUrl,
        //        Name = product.Name,
        //        Price = product.Price,
        //        Stock = product.Stock,
        //        User = product.User
        //    };
        //}

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

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "products");
                    }

                    var product = _converterHelper.ToProduct(model, path, false); // o bool é false porque não é novo (edit)

                    
                    product.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);  // dá o utilizador que estiver "logado"
                    await _productRepository.UpdateAsync(product); // faz o update do produto
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _productRepository.ExistAsync(model.Id)) // verifica se o id existe devido a alguem entretanto ter apagado este produto
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
        [Authorize]
        public async Task<IActionResult> Delete(int? id) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
        {
            if (id == null)
            {
                // return NotFound();
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view
                                                                  // genérico dá para produtos, clientes, fornecedores, etc
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                // return NotFound();
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view
                                                                  // genérico dá para produtos, clientes, fornecedores, etc
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

        public IActionResult ProductNotFound()
        {
            return View();
        }
    }
}
