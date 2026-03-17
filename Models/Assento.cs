using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Cinema.Models
{
    public class Assento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o código da poltrona")]
        [MaxLength(3)]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        public int IdSala { get; set; }

        [ForeignKey("IdSala")]
        public Sala Sala { get; set; }
    }
}
