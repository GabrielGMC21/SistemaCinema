using Microsoft.EntityFrameworkCore;
using Sistema_Cinema.Models;

namespace Sistema_Cinema
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }

        public DbSet<Filme> Filmes { get; set; }
        public DbSet<Assento> Assentos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Cupom> Cupons { get; set; }
        public DbSet<ItemCompra> ItemCompras { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Sessao> Sessoes { get; set; }
        public DbSet<CartaoFidelidade> CartoesFidelidade { get; set; }
        public DbSet<HistoricoFidelidade> HistoricosFidelidade { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }

    }
}
