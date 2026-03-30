using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using Sistema_Cinema;
using Sistema_Cinema.Models;

namespace Sistema_Cinema.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly Contexto _context;

        public ProdutosController(Contexto context)
        {
            _context = context;
        }

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Produtos.ToListAsync());
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,QuantidadeDisponivel,Preco,Categoria,UrlImagem")] Produto produto, IFormFile imagemProduto)
        {
            if (imagemProduto == null || imagemProduto.Length == 0)
            {
                ModelState.AddModelError("UrlImagem", "A imagem do produto é obrigatória");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "produtos");
                    var extensao = Path.GetExtension(imagemProduto.FileName);
                    var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
                    var caminhoCompleto = Path.Combine(wwwrootPath, nomeArquivo);

                    using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await imagemProduto.CopyToAsync(stream);
                    }

                    produto.UrlImagem = $"/images/produtos/{nomeArquivo}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("imagemProduto", $"Erro ao salvar a imagem: {ex.Message}");
                    return View(produto);
                }

                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

        // POST: Produtos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,QuantidadeDisponivel,Preco,Categoria,UrlImagem")] Produto produto, IFormFile imagemProduto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            var produtoExistente = await _context.Produtos.FindAsync(id);
            if (produtoExistente == null)
            {
                return NotFound();
            }

            if (imagemProduto != null && imagemProduto.Length > 0)
            {
                try
                {
                    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    var extensao = Path.GetExtension(imagemProduto.FileName);
                    var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
                    var caminhoCompleto = Path.Combine(wwwrootPath, nomeArquivo);

                    using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await imagemProduto.CopyToAsync(stream);
                    }

                    produto.UrlImagem = $"/images/produtos/{nomeArquivo}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("imagemProduto", $"Erro ao salvar a imagem: {ex.Message}");
                    return View(produto);
                }
            }
            else
            {
                produto.UrlImagem = produtoExistente.UrlImagem;
                ModelState.Remove("imagemProduto");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(produtoExistente).CurrentValues.SetValues(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
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
            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                if (!string.IsNullOrEmpty(produto.UrlImagem))
                {
                    var caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", produto.UrlImagem.TrimStart('/'));
                    if (System.IO.File.Exists(caminhoImagem))
                    {
                        System.IO.File.Delete(caminhoImagem);
                    }
                }
                _context.Produtos.Remove(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
