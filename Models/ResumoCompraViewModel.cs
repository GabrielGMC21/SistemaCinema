using System.Collections.Generic;

namespace Sistema_Cinema.Models
{
    public class ProdutoPedido
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
    }

    public class ResumoCompraViewModel
    {
        public Sessao Sessao { get; set; }
        public List<AssentoViewModel> Assentos { get; set; } = new List<AssentoViewModel>();
        public decimal SubtotalIngressos { get; set; }
        public List<Produto> Produtos { get; set; } = new List<Produto>();
        public List<ProdutoPedido> ProdutosPedidos { get; set; } = new List<ProdutoPedido>();
        public string CupomCodigo { get; set; }
    }
}
