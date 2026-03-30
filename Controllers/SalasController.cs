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
    public class SalasController : Controller
    {
        private readonly Contexto _context;

        public SalasController(Contexto context)
        {
            _context = context;
        }

        // GET: Salas
        public async Task<IActionResult> Index()
        {
            var salas = await _context.Salas
                .Include(s => s.Assentos)
                .ToListAsync();

            return View(salas);
        }

        // GET: Salas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas
                .Include(s => s.Assentos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        // GET: Salas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Salas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumeroSala")] Sala sala, int? linhas, int? assentosPorLinha)
        {
            if(_context.Salas.Any(s => s.NumeroSala == sala.NumeroSala))
            {
                ModelState.AddModelError("NumeroSala", "Já existe uma sala com esse número.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(sala);
                await _context.SaveChangesAsync();

                int quantidadeLinhas = linhas ?? 0;
                int quantidadeAssentosPorLinha = assentosPorLinha ?? 0;
                if (quantidadeLinhas > 0 && quantidadeAssentosPorLinha > 0)
                {
                    if (quantidadeLinhas > 26) quantidadeLinhas = 26;
                    if (quantidadeAssentosPorLinha > 100) quantidadeAssentosPorLinha = 100;

                    var assentos = new List<Assento>();
                    for (int i = 0; i < quantidadeLinhas; i++)
                    {
                        char rowChar = (char)('A' + i);
                        for (int j = 1; j <= quantidadeAssentosPorLinha; j++)
                        {
                            assentos.Add(new Assento
                            {
                                Codigo = $"{rowChar}{j}",
                                IdSala = sala.Id
                            });
                        }
                    }

                    _context.Assentos.AddRange(assentos);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(sala);
        }

        // GET: Salas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas.FindAsync(id);
            if (sala == null)
            {
                return NotFound();
            }
            return View(sala);
        }

        // POST: Salas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroSala")] Sala sala)
        {
            if (id != sala.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sala);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaExists(sala.Id))
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
            return View(sala);
        }

        // GET: Salas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        // POST: Salas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala != null)
            {
                _context.Salas.Remove(sala);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaExists(int id)
        {
            return _context.Salas.Any(e => e.Id == id);
        }
    }
}
