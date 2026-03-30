using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Cinema;
using Sistema_Cinema.Models;

namespace Sistema_Cinema.Controllers
{
    public class AdminController : Controller
    {
        private readonly Contexto _context;

        public AdminController(Contexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> MovimentosCartao()
        {
            var movimentos = await _context.HistoricosFidelidade
                .Include(h => h.CartaoFidelidade)
                    .ThenInclude(c => c.Cliente)
                        .ThenInclude(cl => cl.AppUser)
                .OrderByDescending(h => h.DataMovimento)
                .ToListAsync();

            return View(movimentos);
        }

        public async Task<IActionResult> Compras()
        {
            var compras = await _context.Compras
                .Include(c => c.Cliente)
                    .ThenInclude(cl => cl.AppUser)
                .Include(c => c.Cupom)
                .Include(c => c.ItemCompras)
                    .ThenInclude(i => i.Produto)
                .OrderByDescending(c => c.DataCompra)
                .ToListAsync();

            return View(compras);
        }
    }
}
