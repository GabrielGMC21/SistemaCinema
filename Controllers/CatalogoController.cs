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
    public class CatalogoController : Controller
    {
        private readonly Contexto _context;

        public CatalogoController(Contexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var filmes = await _context.Filmes
                .Where(f => f.emCartaz && f.Sessoes.Any(s => s.DataHora > DateTime.Now))
                .OrderBy(f => f.Titulo)
                .ToListAsync();

            var filmesComMedia = new List<dynamic>();
            foreach (var filme in filmes)
            {
                var avaliacoes = await _context.Avaliacoes
                    .Where(a => a.IdFilme == filme.Id)
                    .ToListAsync();

                var mediaAvaliacao = avaliacoes.Any() ? Math.Round(avaliacoes.Average(a => a.Nota), 1) : 0;
                var totalAvaliacoes = avaliacoes.Count;

                var filmeComMedia = new
                {
                    Filme = filme,
                    MediaAvaliacao = mediaAvaliacao,
                    TotalAvaliacoes = totalAvaliacoes
                };

                filmesComMedia.Add(filmeComMedia);
            }
            
            return View(filmesComMedia);
        }

        public async Task<IActionResult> Detalhes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .Include(f => f.Sessoes.Where(s => s.DataHora > DateTime.Now))
                    .ThenInclude(s => s.Sala)
                        .ThenInclude(sala => sala.Assentos)
                .FirstOrDefaultAsync(m => m.Id == id && m.emCartaz);

            if (filme == null)
            {
                return NotFound();
            }

            if (filme.Sessoes != null)
            {
                filme.Sessoes = filme.Sessoes.OrderBy(s => s.DataHora).ToList();
            }

            var avaliacoes = await _context.Avaliacoes
                .Where(a => a.IdFilme == id)
                .Include(a => a.Cliente)
                    .ThenInclude(c => c.AppUser)
                .OrderByDescending(a => a.DataAvaliacao)
                .ToListAsync();

            var mediaAvaliacao = avaliacoes.Any() ? (decimal)Math.Round(avaliacoes.Average(a => a.Nota), 1) : 0M;

            var podeAvaliar = false;
            Avaliacao? minhaAvaliacao = null;

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(appUserId))
                {
                    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.AppUserId == appUserId);
                    
                    if (cliente != null)
                    {
                        var temReservaConfirmada = await _context.Reservas
                            .Include(r => r.Sessao)
                            .Include(r => r.Compra)
                            .Where(r => r.Sessao!.IdFilme == id && r.Compra!.Cliente!.Id == cliente.Id)
                            .AnyAsync();

                        podeAvaliar = temReservaConfirmada;

                        minhaAvaliacao = await _context.Avaliacoes
                            .FirstOrDefaultAsync(a => a.IdCliente == cliente.Id && a.IdFilme == id);
                    }
                }
            }

            var viewModel = new FilmeDetalhesViewModel
            {
                Filme = filme,
                Avaliacoes = avaliacoes,
                MediaAvaliacao = mediaAvaliacao,
                TotalAvaliacoes = avaliacoes.Count,
                PodeAvaliar = podeAvaliar,
                MinhaAvaliacao = minhaAvaliacao
            };

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> AssentosDisponiveis(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessao = await _context.Sessoes
                .Include(s => s.Filme)
                .Include(s => s.Sala)
                    .ThenInclude(sala => sala.Assentos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sessao == null)
            {
                return NotFound();
            }

            var reservasAssentoIds = await _context.Reservas
                .Where(r => r.IdSessao == id)
                .Select(r => r.IdAssento)
                .ToListAsync();

            var assentosVm = sessao.Sala.Assentos
                .OrderBy(a => a.Codigo)
                .Select(a => new AssentoViewModel
                {
                    Id = a.Id,
                    Codigo = a.Codigo,
                    Reservado = reservasAssentoIds.Contains(a.Id)
                })
                .ToList();

            var vm = new AssentosParaReservaViewModel
            {
                Sessao = sessao,
                Assentos = assentosVm
            };

            return View("AssentosDisponiveis", vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ResumoCompra(int sessaoId, List<int> assentoIds)
        {
            if (sessaoId <= 0 || assentoIds == null || !assentoIds.Any())
            {
                return RedirectToAction("AssentosDisponiveis", new { id = sessaoId });
            }

            var sessao = await _context.Sessoes
                .Include(s => s.Filme)
                .Include(s => s.Sala)
                    .ThenInclude(sala => sala.Assentos)
                .FirstOrDefaultAsync(s => s.Id == sessaoId);

            if (sessao == null)
                return NotFound();

            var assentosVm = sessao.Sala.Assentos
                .Where(a => assentoIds.Contains(a.Id))
                .OrderBy(a => a.Codigo)
                .Select(a => new AssentoViewModel
                {
                    Id = a.Id,
                    Codigo = a.Codigo,
                    Reservado = false
                })
                .ToList();

            var produtos = await _context.Produtos.Where(p => p.QuantidadeDisponivel > 0).OrderBy(p => p.Nome).ToListAsync();

            var vm = new ResumoCompraViewModel
            {
                Sessao = sessao,
                Assentos = assentosVm,
                SubtotalIngressos = sessao.PrecoBase * assentosVm.Count,
                Produtos = produtos
            };

            return View("ResumoCompra", vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmarReserva(int sessaoId, List<int> assentoIds, string cupomCodigo, List<ProdutoPedido> produtos)
        {
            var sessao = await _context.Sessoes
                .Include(s => s.Sala)
                .FirstOrDefaultAsync(s => s.Id == sessaoId);

            if (sessao == null)
                return NotFound();

            if (assentoIds == null || !assentoIds.Any())
            {
                TempData["ErroReserva"] = "Nenhum assento selecionado.";
                return RedirectToAction("AssentosDisponiveis", new { id = sessaoId });
            }

            var reservasExistentes = await _context.Reservas
                .Where(r => r.IdSessao == sessaoId && assentoIds.Contains(r.IdAssento))
                .Select(r => r.IdAssento)
                .ToListAsync();

            if (reservasExistentes.Any())
            {
                TempData["ErroReserva"] = $"Assentos já reservados: {string.Join(',', reservasExistentes)}";
                return RedirectToAction("AssentosDisponiveis", new { id = sessaoId });
            }

            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId))
                return Challenge();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.AppUserId == appUserId);
            if (cliente == null)
            {
                cliente = new Cliente { AppUserId = appUserId, DataCadastro = DateTime.Now };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }

            decimal valorIngressos = sessao.PrecoBase * assentoIds.Count;
            decimal valorProdutos = 0M;
            var itensCompra = new List<ItemCompra>();

            if (produtos != null && produtos.Any())
            {
                var selecionados = produtos.Where(p => p.Quantidade > 0).ToList();
                if (selecionados.Any())
                {
                    var produtoIds = selecionados.Select(p => p.Id).ToList();
                    var produtoObjs = await _context.Produtos.Where(p => produtoIds.Contains(p.Id)).ToListAsync();
                    foreach (var sel in selecionados)
                    {
                        var prod = produtoObjs.FirstOrDefault(p => p.Id == sel.Id);
                        if(prod.QuantidadeDisponivel < sel.Quantidade)
                        {
                            TempData["ErroReserva"] = $"Produto {prod.Nome} não tem estoque suficiente. Disponível: {prod.QuantidadeDisponivel}";
                            return RedirectToAction("AssentosDisponiveis", new { id = sessaoId });
                        }else
                        {
                            prod.QuantidadeDisponivel -= sel.Quantidade;
                            if (prod != null)
                            {
                                var subtotal = prod.Preco * sel.Quantidade;
                                valorProdutos += subtotal;
                                itensCompra.Add(new ItemCompra { IdProduto = prod.Id, Quantidade = sel.Quantidade, Subtotal = subtotal });
                            }
                            _context.Produtos.Update(prod);
                        }
                    }
                }
            }

            decimal totalSemDesconto = valorIngressos + valorProdutos;
            decimal desconto = 0M;
            Cupom? cupom = null;

            if (!string.IsNullOrEmpty(cupomCodigo))
            {
                cupom = await _context.Cupons.FirstOrDefaultAsync(c => c.Codigo == cupomCodigo && c.Ativo);
                if (cupom == null)
                {
                    TempData["ErroReserva"] = "Cupom inválido.";
                    return RedirectToAction("AssentosDisponiveis", new { id = sessaoId });
                }
                if (cupom.DataVencimento != null && cupom.DataVencimento < DateTime.Now)
                {
                    TempData["ErroReserva"] = "Cupom expirado.";
                    return RedirectToAction("AssentosDisponiveis", new { id = sessaoId });
                }
                if (totalSemDesconto < cupom.ValorMinimo)
                {
                    TempData["ErroReserva"] = $"Cupom exige compra mínima de {cupom.ValorMinimo:C}.";
                    return RedirectToAction("AssentosDisponiveis", new { id = sessaoId });
                }

                cupom.Ativo = false;
                _context.Cupons.Update(cupom);

                desconto = Decimal.Round(totalSemDesconto * (cupom.PercentualDesconto / 100M), 2);
            }

            var compra = new Compra
            {
                Cliente = cliente,
                DataCompra = DateTime.Now,
                ValorIngressos = valorIngressos,
                ValorDesconto = desconto,
                ValorFinal = totalSemDesconto - desconto,
            };

            if (cupom != null)
            {
                compra.Cupom = cupom;
            }

            foreach (var item in itensCompra)
            {
                item.Compra = compra;
                compra.ItemCompras.Add(item);
                _context.ItemCompras.Add(item);
            }

            foreach (var assentoId in assentoIds)
            {
                var reserva = new Reserva { IdSessao = sessaoId, IdAssento = assentoId, Compra = compra, DataReserva = DateTime.Now };
                compra.Reservas.Add(reserva);
                _context.Reservas.Add(reserva);
            }

            _context.Compras.Add(compra);

            var cartaoFidelidade = await _context.CartoesFidelidade.FirstOrDefaultAsync(c => c.IdCliente == cliente.Id);
            if (cartaoFidelidade == null)
            {
                cartaoFidelidade = new CartaoFidelidade { IdCliente = cliente.Id, SaldoPontos = 0 };
                _context.CartoesFidelidade.Add(cartaoFidelidade);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            
            int pontos = 0;
            if (compra.ValorFinal >= 10)
            {
                pontos = (int)(compra.ValorFinal * 0.1m);
                cartaoFidelidade.SaldoPontos += pontos;
                _context.CartoesFidelidade.Update(cartaoFidelidade);
                var historicoFidelidade = new HistoricoFidelidade
                {
                    IdCartao = cartaoFidelidade.Id,
                    Tipo = HistoricoFidelidade.TipoMovimento.Ganhos,
                    Quantidade = pontos,
                    DataMovimento = DateTime.Now,
                    Descricao = $"Ganho de {pontos} pontos pela compra #{compra.Id} - Valor final: {compra.ValorFinal:C}"
                };
                _context.HistoricosFidelidade.Add(historicoFidelidade);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ReservaConfirmada", new { id = compra.Id, pontos });
        }

        [Authorize]
        public async Task<IActionResult> ReservaConfirmada(int id, int? pontos)
        {
            var compra = await _context.Compras
                .Include(c => c.Reservas)
                    .ThenInclude(r => r.Sessao)
                        .ThenInclude(s => s.Filme)
                .Include(c => c.Reservas)
                    .ThenInclude(r => r.Sessao) 
                        .ThenInclude(s => s.Sala)
                .Include(c => c.Reservas)
                    .ThenInclude(r => r.Assento)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
                return NotFound();

            if (pontos != null)
            {
                ViewBag.Pontos = pontos;
            }
            return View(compra);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CriarAvaliacao(int filmeId, int nota, string comentario)
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId))
                return Challenge();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.AppUserId == appUserId);
            if (cliente == null)
            {
                TempData["ErroAvaliacao"] = "Cliente não encontrado.";
                return RedirectToAction("Detalhes", new { id = filmeId });
            }

            var avaliacaoExistente = await _context.Avaliacoes
                .FirstOrDefaultAsync(a => a.IdCliente == cliente.Id && a.IdFilme == filmeId);

            if (avaliacaoExistente != null)
            {
                TempData["ErroAvaliacao"] = "Você já avaliou este filme.";
                return RedirectToAction("Detalhes", new { id = filmeId });
            }

            var avaliacao = new Avaliacao
            {
                IdCliente = cliente.Id,
                IdFilme = filmeId,
                Nota = nota,
                Comentario = comentario,
                DataAvaliacao = DateTime.Now
            };

            _context.Avaliacoes.Add(avaliacao);
            await _context.SaveChangesAsync();

            TempData["SucessoAvaliacao"] = "Avaliação criada com sucesso!";
            return RedirectToAction("Detalhes", new { id = filmeId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditarAvaliacao(int filmeId, int avaliacaoId, int nota, string comentario)
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId))
                return Challenge();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.AppUserId == appUserId);
            if (cliente == null)
            {
                TempData["ErroAvaliacao"] = "Cliente não encontrado.";
                return RedirectToAction("Detalhes", new { id = filmeId });
            }

            var avaliacao = await _context.Avaliacoes
                .FirstOrDefaultAsync(a => a.Id == avaliacaoId && a.IdCliente == cliente.Id);

            if (avaliacao == null)
            {
                TempData["ErroAvaliacao"] = "Avaliação não encontrada.";
                return RedirectToAction("Detalhes", new { id = filmeId });
            }

            avaliacao.Nota = nota;
            avaliacao.Comentario = comentario;
            avaliacao.DataAvaliacao = DateTime.Now;

            _context.Avaliacoes.Update(avaliacao);
            await _context.SaveChangesAsync();

            TempData["SucessoAvaliacao"] = "Avaliação atualizada com sucesso!";
            return RedirectToAction("Detalhes", new { id = filmeId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeletarAvaliacao(int filmeId, int avaliacaoId)
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId))
                return Challenge();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.AppUserId == appUserId);
            if (cliente == null)
            {
                TempData["ErroAvaliacao"] = "Cliente não encontrado.";
                return RedirectToAction("Detalhes", new { id = filmeId });
            }

            var avaliacao = await _context.Avaliacoes
                .FirstOrDefaultAsync(a => a.Id == avaliacaoId && a.IdCliente == cliente.Id);

            if (avaliacao == null)
            {
                TempData["ErroAvaliacao"] = "Avaliação não encontrada.";
                return RedirectToAction("Detalhes", new { id = filmeId });
            }

            _context.Avaliacoes.Remove(avaliacao);
            await _context.SaveChangesAsync();

            TempData["SucessoAvaliacao"] = "Avaliação deletada com sucesso!";
            return RedirectToAction("Detalhes", new { id = filmeId });
        }
    }
}
