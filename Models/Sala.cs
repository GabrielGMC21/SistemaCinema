using System.ComponentModel.DataAnnotations;

namespace Sistema_Cinema.Models
{
    public class Sala
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="Obrigatório informar número da Sala")]
        [Display(Name = "Número da Sala")]
        public int NumeroSala { get; set; }

        public ICollection<Assento> Assentos { get; set; } = new List<Assento>();

        public int Capacidade => Assentos.Count;
    }
}
