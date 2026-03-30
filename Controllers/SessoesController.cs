using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Sistema_Cinema;
using Sistema_Cinema.Models;

namespace Sistema_Cinema.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SessoesController : Controller
    {
        private readonly Contexto _context;

        public SessoesController(Contexto context)
        {
            _context = context;
        }

        // GET: Sessoes
        public async Task<IActionResult> Index()
        {
            var contexto = _context.Sessoes
                .Include(s => s.Filme)
                .Include(s => s.Sala)
                .ThenInclude(sala => sala.Assentos);
            return View(await contexto.ToListAsync());
        }

        // GET: Sessoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessao = await _context.Sessoes
                .Include(s => s.Filme)
                .Include(s => s.Sala)
                .ThenInclude(sala => sala.Assentos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sessao == null)
            {
                return NotFound();
            }

            return View(sessao);
        }

        // GET: Sessoes/Create
        public IActionResult Create()
        {
            ViewData["IdFilme"] = new SelectList(_context.Filmes, "Id", "Titulo");
            ViewData["IdSala"] = new SelectList(_context.Salas, "Id", "NumeroSala");
            return View();
        }

        // POST: Sessoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataHora,PrecoBase,IdFilme,IdSala")] Sessao sessao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sessao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            ViewData["IdFilme"] = new SelectList(_context.Filmes, "Id", "Titulo", sessao.IdFilme);
            ViewData["IdSala"] = new SelectList(_context.Salas, "Id", "NumeroSala", sessao.IdSala);
            return View(sessao);
        }

        // GET: Sessoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessao = await _context.Sessoes.FindAsync(id);
            if (sessao == null)
            {
                return NotFound();
            }
            ViewData["IdFilme"] = new SelectList(_context.Filmes, "Id", "Titulo", sessao.IdFilme);
            ViewData["IdSala"] = new SelectList(_context.Salas, "Id", "NumeroSala", sessao.IdSala);
            return View(sessao);
        }

        // POST: Sessoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataHora,PrecoBase,IdFilme,IdSala")] Sessao sessao)
        {
            if (id != sessao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sessao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessaoExists(sessao.Id))
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
            ViewData["IdFilme"] = new SelectList(_context.Filmes, "Id", "Titulo", sessao.IdFilme);
            ViewData["IdSala"] = new SelectList(_context.Salas, "Id", "NumeroSala", sessao.IdSala);
            return View(sessao);
        }

        // GET: Sessoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessao = await _context.Sessoes
                .Include(s => s.Filme)
                .Include(s => s.Sala)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sessao == null)
            {
                return NotFound();
            }

            return View(sessao);
        }

        // POST: Sessoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sessao = await _context.Sessoes.FindAsync(id);
            if (sessao != null)
            {
                _context.Sessoes.Remove(sessao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessaoExists(int id)
        {
            return _context.Sessoes.Any(e => e.Id == id);
        }
    }
}
