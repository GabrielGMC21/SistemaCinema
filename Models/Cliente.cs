using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Cinema.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AppUserId { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public ICollection<Compra> HistoricoCompras { get; set; } = new List<Compra>();
        public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
        public CartaoFidelidade CartaoFidelidade { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }
    }
}
