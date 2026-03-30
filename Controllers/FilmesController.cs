using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Sistema_Cinema;
using Sistema_Cinema.Models;

namespace Sistema_Cinema.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FilmesController : Controller
    {
        private readonly Contexto _context;

        public FilmesController(Contexto context)
        {
            _context = context;
        }

        // GET: Filmes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Filmes.ToListAsync());
        }

        // GET: Filmes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // GET: Filmes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Filmes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Diretor,Genero,Ano,Classificacao,Duracao,Sinopse,DataCadastro,emCartaz,DuracaoHoras,DuracaoMinutos")] Filme filme, IFormFile imagemPoster)
        {
            int horas = filme.DuracaoHoras ?? 0;
            int minutos = filme.DuracaoMinutos ?? 0;

            if (horas == 0 && minutos == 0)
            {
                ModelState.AddModelError("DuracaoHoras", "A duração do filme deve ser maior que 0");
            }

            if (imagemPoster == null || imagemPoster.Length == 0)
            {
                ModelState.AddModelError("UrlPoster", "O poster é obrigatório");
            }

            if (ModelState.IsValid)
            {
                filme.Duracao = (horas * 60) + minutos;
            
                try
                {
                    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    var extensao = Path.GetExtension(imagemPoster.FileName);
                    var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
                    var caminhoCompleto = Path.Combine(wwwrootPath, nomeArquivo);

                    using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await imagemPoster.CopyToAsync(stream);
                    }

                    filme.UrlPoster = $"/images/{nomeArquivo}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("imagemPoster", $"Erro ao salvar a imagem: {ex.Message}");
                    return View(filme);
                }

                _context.Add(filme);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filme);
        }

        // GET: Filmes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes.FindAsync(id);
            if (filme == null)
            {
                return NotFound();
            }
            filme.DuracaoHoras = filme.Duracao / 60;
            filme.DuracaoMinutos = filme.Duracao % 60;

            return View(filme);
        }

        // POST: Filmes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Diretor,Genero,Ano,Classificacao,Duracao,Sinopse,DataCadastro,emCartaz,DuracaoHoras,DuracaoMinutos")] Filme filme, IFormFile imagemPoster)
        {
            if (id != filme.Id)
            {
                return NotFound();
            }

            var filmeExistente = await _context.Filmes.FindAsync(id);
            if (filmeExistente == null)
            {
                return NotFound();
            }

            int horas = filme.DuracaoHoras ?? 0;
            int minutos = filme.DuracaoMinutos ?? 0;

            if (horas == 0 && minutos == 0)
            {
                ModelState.AddModelError("DuracaoHoras", "A duração do filme deve ser maior que 0");
                return View(filme);
            }

            filme.Duracao = (horas * 60) + minutos;

            if (imagemPoster != null && imagemPoster.Length > 0)
            {
                if (!string.IsNullOrEmpty(filmeExistente.UrlPoster))
                {
                    var caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filmeExistente.UrlPoster.TrimStart('/'));
                    if (System.IO.File.Exists(caminhoImagem))
                    {
                        System.IO.File.Delete(caminhoImagem);
                    }
                }

                try
                {
                    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    var extensao = Path.GetExtension(imagemPoster.FileName);
                    var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
                    var caminhoCompleto = Path.Combine(wwwrootPath, nomeArquivo);

                    using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await imagemPoster.CopyToAsync(stream);
                    }

                    filme.UrlPoster = $"/images/{nomeArquivo}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("imagemPoster", $"Erro ao salvar a imagem: {ex.Message}");
                    return View(filme);
                }
            }
            else
            {
                filme.UrlPoster = filmeExistente.UrlPoster;
                ModelState.Remove("imagemPoster");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(filmeExistente).CurrentValues.SetValues(filme);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmeExists(filme.Id))
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
            return View(filme);
        }

        // GET: Filmes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // POST: Filmes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filme = await _context.Filmes.FindAsync(id);
            if (filme != null)
            {
                if (!string.IsNullOrEmpty(filme.UrlPoster))
                {
                    var caminhoImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filme.UrlPoster.TrimStart('/'));
                    if (System.IO.File.Exists(caminhoImagem))
                    {
                        System.IO.File.Delete(caminhoImagem);
                    }
                }
                _context.Filmes.Remove(filme);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmeExists(int id)
        {
            return _context.Filmes.Any(e => e.Id == id);
        }
    }
}
