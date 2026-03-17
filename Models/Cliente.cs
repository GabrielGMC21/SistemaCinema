using System.ComponentModel.DataAnnotations;

namespace Sistema_Cinema.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o nome")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 50 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o e-mail")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a senha")]
        public string SenhaHash { get; set; }

        public ICollection<Compra> HistoricoCompras { get; set; } = new List<Compra>();
        public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
        public CartaoFidelidade CartaoFidelidade { get; set; }
    }
}
