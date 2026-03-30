using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Cinema.Models
{
    public class HistoricoFidelidade
    {
        public enum TipoMovimento
        {
            Ganhos,

            Utilizados
        }

        [Key]
        public int Id { get; set; }

        public int IdCartao { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o tipo de movimento")]
        [Display(Name = "Tipo de Movimento")]
        public TipoMovimento Tipo { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a quantidade")]
        [Range(1, 9999999, ErrorMessage = "A quantidade deve ser maior que 0")]
        [Display(Name = "Quantidade de Pontos")]
        public int Quantidade { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(500)]
        public string Descricao { get; set; }

        [Display(Name = "Data do Movimento")]
        public DateTime DataMovimento { get; set; } = DateTime.Now;

        [ForeignKey("IdCartao")]
        public CartaoFidelidade CartaoFidelidade { get; set; }
    }
}
