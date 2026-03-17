using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Cinema.Models
{
    public class CartaoFidelidade
    {
        [Key]
        public int Id { get; set; }

        public int IdCliente { get; set; }

        [Display(Name = "Saldo de Pontos")]
        [Range(0, 9999999, ErrorMessage = "O saldo não pode ser negativo")]
        public int SaldoPontos { get; set; } = 0;

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public ICollection<HistoricoFidelidade> Historico { get; set; } = new List<HistoricoFidelidade>();

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }
    }
}
