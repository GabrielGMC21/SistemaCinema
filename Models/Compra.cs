using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Cinema.Models
{
    public class Compra
    {
        [Key]
        public int Id { get; set; }

        public int IdCliente { get; set; }
        public int? IdCupom { get; set; }

        [Display(Name = "Data da Compra")]
        public DateTime DataCompra { get; set; }

        [Display(Name = "Valor dos ingressos")]
        public decimal ValorIngressos { get; set; }

        [Display(Name = "Valor do Desconto")]
        public decimal ValorDesconto { get; set; }

        [Display(Name = "Valor Final")]
        public decimal ValorFinal { get; set; }

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public ICollection<ItemCompra> ItemCompras { get; set; } = new List<ItemCompra>();

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }

        [ForeignKey("IdCupom")]
        public Cupom Cupom { get; set; }
    }
}
