using System.ComponentModel.DataAnnotations;

namespace Sistema_Cinema.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o nome do produto")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 250 caracteres")]
        public string Nome { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        public string Descricao { get; set; }

        [Display(Name = "Quantidade disponível")]
        [Required(ErrorMessage = "Obrigatório informar a quantidade disponível")]
        [Range(0, 9999, ErrorMessage = "A quantidade disponível deve ser um valor positivo até 9999")]
        public int QuantidadeDisponivel { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o valor do produto")]
        [Range(0, 9999.99, ErrorMessage = "Obrigatório valor ser positivo até 9999,99")]
        [Display(Name = "Preço")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Preco { get; set; }

        [Display(Name = "Categoria")]
        [StringLength(100, ErrorMessage = "A categoria não pode exceder 100 caracteres")]
        public string Categoria { get; set; }

        [Display(Name = "URL da Imagem")]
        [StringLength(500)]
        public string? UrlImagem { get; set; }
    }
}
