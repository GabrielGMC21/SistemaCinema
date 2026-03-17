using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Cinema.Models
{
    public class Avaliacao
    {
        [Key]
        public int Id { get; set; }

        public int IdCliente { get; set; }
        public int IdFilme { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a nota")]
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5 estrelas")]
        [Display(Name = "Classificação (estrelas)")]
        public int Nota { get; set; }

        [Display(Name = "Comentário")]
        [StringLength(1000, ErrorMessage = "O comentário não pode exceder 1000 caracteres")]
        public string Comentario { get; set; }

        [Display(Name = "Data da Avaliação")]
        public DateTime DataAvaliacao { get; set; } = DateTime.Now;

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }

        [ForeignKey("IdFilme")]
        public Filme Filme { get; set; }
    }
}
