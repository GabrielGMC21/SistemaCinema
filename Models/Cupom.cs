using System.ComponentModel.DataAnnotations;

namespace Sistema_Cinema.Models
{
    public class Cupom
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o código do cupom.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O código deve ter entre 3 e 50 caracteres")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o porcentual de desconto.")]
        [Range(1, 100, ErrorMessage = "O desconto deve ser entre 1% e 100%")]
        [Display(Name = "Percentual de desconto")]
        public decimal PercentualDesconto { get; set; }

        [Display(Name = "Valor Mínimo da Compra")]
        [Range(0, 9999.99, ErrorMessage = "Obrigatório valor ser positivo até 9999,99")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ValorMinimo { get; set; } = 0;

        [Display(Name = "Data de Vencimento")]
        public DateTime? DataVencimento { get; set; }

        [Required(ErrorMessage = "Obrigatório informar se o cupom é ativo")]
        public bool Ativo { get; set; }
    }
}
