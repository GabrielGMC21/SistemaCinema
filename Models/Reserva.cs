using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Cinema.Models
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        public int IdSessao { get; set; }
        public int IdAssento { get; set; }
        public int IdCompra { get; set; }

        [Display(Name = "Data da Reserva")]
        public DateTime DataReserva { get; set; } = DateTime.Now;

        [ForeignKey("IdAssento")]
        public Assento? Assento { get; set; }

        [ForeignKey("IdSessao")]
        public Sessao? Sessao { get; set; }

        [ForeignKey("IdCompra")]
        public Compra? Compra { get; set; }
    } 
}
