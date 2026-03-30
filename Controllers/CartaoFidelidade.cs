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
    [Authorize]
    public class CartaoFidelidadeController : Controller
    {
        private readonly Contexto _context;

        public CartaoFidelidadeController(Contexto context)
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
                .Include(c => c.CartaoFidelidade)
                    .ThenInclude(cf => cf.Historico)
                .FirstOrDefaultAsync(c => c.AppUserId == appUserId);

            if(cliente == null || cliente.CartaoFidelidade == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        public async Task<IActionResult> Resgatar(int valorPontos)
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId))
            {
                return Challenge();
            }

            var cliente = await _context.Clientes
                .Include(c => c.CartaoFidelidade)
                .FirstOrDefaultAsync(c => c.AppUserId == appUserId);

            if (cliente == null || cliente.CartaoFidelidade == null)
            {
                return NotFound();
            }

            var cartaoFidelidade = cliente.CartaoFidelidade;

            if (valorPontos % 10 != 0)
            {
                TempData["MensagemErro"] = "O valor deve ser um múltiplo de 10.";
                return RedirectToAction(nameof(Index));
            }

            if (cartaoFidelidade.SaldoPontos < valorPontos)
            {
                TempData["MensagemErro"] = "Saldo insuficiente de pontos.";
                return RedirectToAction(nameof(Index));
            }

            decimal percentualDesconto = (decimal)valorPontos / 10;
            if (percentualDesconto > 50)
            {
                TempData["MensagemErro"] = "Valor máximo de desconto excedido.";
                return RedirectToAction(nameof(Index));
            }

            string codigoCupom = "FIDELIDADE_" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            var cupom = new Cupom
            {
                Codigo = codigoCupom,
                PercentualDesconto = percentualDesconto,
                Ativo = true,
                DataVencimento = DateTime.Now.AddMonths(3) 
            };

            _context.Cupons.Add(cupom);

            cartaoFidelidade.SaldoPontos -= valorPontos;
            _context.CartoesFidelidade.Update(cartaoFidelidade);

            var historicoFideliade = new HistoricoFidelidade
            {
                IdCartao = cartaoFidelidade.Id,
                Tipo = HistoricoFidelidade.TipoMovimento.Utilizados,
                Quantidade = valorPontos,
                DataMovimento = DateTime.Now,
                Descricao = $"Resgate de {valorPontos} pontos por cupom {codigoCupom} com {percentualDesconto}% de desconto"
            };

            _context.HistoricosFidelidade.Add(historicoFideliade);
            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = $"Cupom resgatado com sucesso! Código: {codigoCupom} - Desconto: {percentualDesconto}%";

            return RedirectToAction(nameof(Index));
        }
    }
}
