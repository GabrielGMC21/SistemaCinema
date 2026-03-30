using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Sistema_Cinema;
using Sistema_Cinema.Models;

namespace Sistema_Cinema.Controllers
{
    public class ProfileController : Controller
    {
        private readonly Contexto _context;

        public ProfileController(Contexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId))
            {
                return Challenge();
            }

            var cliente = await _context.Clientes
                .Include(c => c.HistoricoCompras)
                    .ThenInclude(comp => comp.Reservas)
                        .ThenInclude(r => r.Assento)
                        .ThenInclude(a => a.Sala)
                .Include(c => c.HistoricoCompras)
                    .ThenInclude(comp => comp.Reservas)
                        .ThenInclude(r => r.Sessao)
                            .ThenInclude(s => s.Filme)
                .Include(c => c.HistoricoCompras)
                    .ThenInclude(comp => comp.ItemCompras)
                        .ThenInclude(ic => ic.Produto)
                .Include(c => c.Avaliacoes)
                    .ThenInclude(a => a.Filme)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.AppUserId == appUserId);
            
            if (cliente == null)
            {
                cliente = new Cliente { AppUserId = appUserId, DataCadastro = DateTime.Now };
                _context.Clientes.Add(cliente);
            }

            return View(cliente);
        }
    }
}
