﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Data.Entities;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IRepository _repository;

        public ProductsController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_repository.GetProducts()); // trás todos os produtos
        }

        // GET: Products/Details/5
        public IActionResult Details(int? id) // pode aceitar null
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _repository.GetProduct(id.Value); // tem que ser id.value para que se for null não "rebentar"
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
                _repository.AddProduct(product); // recebe o produto
                await _repository.SaveAllAsync(); // guarda na base de dados de forma assíncrona
                return RedirectToAction(nameof(Index)); // redireciona para a action index (mostra a lista dos produtos)
            }
            return View(product); // se o produto não passar nas validações mostra a view e deixa ficar lá o produto,
                                  // para o utilizador não ter que preencher tudo de novo
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
            {
                return NotFound();
            }

            var product = _repository.GetProduct(id.Value); // coloca o id em memória e verifica caso o id tenha sido eliminado entretanto
            if (product == null)                            // tem que ser id.value para que se for null não "rebentar"
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
                    _repository.UpdateProduct(product); // faz o update do produto
                    await _repository.SaveAllAsync(); // grava na base de dados
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_repository.ProductExists(product.Id)) // verifica se o id existe devido a alguem entretanto ter apagado este produto
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
        public IActionResult Delete(int? id) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _repository.GetProduct(id.Value);
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
            var product = _repository.GetProduct(id); // o id é verficado para ver se ainda existe
            _repository.RemoveProduct(product); //remover em memória
            await _repository.SaveAllAsync(); // guarda as alterações na base de dados
            return RedirectToAction(nameof(Index));
        }
    }
}
