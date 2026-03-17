using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Cinema.Models
{
    public class Sessao
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="Obrigatório informar a data e hora da sessão")]
        [Display(Name = "Data e hora")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime DataHora { get; set; }

        [Required(ErrorMessage ="Obrigatório informar o valor da sessão")]
        [Range(0, 9999.99, ErrorMessage = "Obrigatório valor ser positivo até 9999,99")]
        [Display(Name = "Preço base")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PrecoBase { get; set; }

        public int IdFilme { get; set; }
        
        public int IdSala { get; set; }

        [ForeignKey("IdSala")]
        public Sala Sala { get; set; }

        [ForeignKey("IdFilme")]
        public Filme Filme { get; set; }

    }
}
