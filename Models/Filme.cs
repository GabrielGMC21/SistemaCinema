using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Cinema.Models
{
    public class Filme
    {
        public enum Classificacoes
        {
            [Display(Name = "Livre para todos os públicos")]
            Livre,

            [Display(Name = "Não recomendado para menores de 10 anos")]
            Classificacao10 = 10,

            [Display(Name = "Não recomendado para menores de 12 anos")]
            Classificacao12 = 12,

            [Display(Name = "Não recomendado para menores de 14 anos")]
            Classificacao14 = 14,

            [Display(Name = "Não recomendado para menores de 16 anos")]
            Classificacao16 = 16,

            [Display(Name = "Não recomendado para menores de 18 anos")]
            Classificacao18 = 18
        }

        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Obrigatório o nome do filme")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }
        [Required(ErrorMessage = "Obrigatório o nome do diretor")]
        public string Diretor { get; set; }
        [Required(ErrorMessage = "Obrigatório informar o gênero")]
        [Display(Name = "Gênero")]
        public string Genero { get; set; }
        [Required(ErrorMessage = "Obrigatório informar o ano")]
        [Range(1800, 2100, ErrorMessage = "O ano deve ser entre 1800 e 2100")]
        public int Ano { get; set; }
        [Required(ErrorMessage = "Obrigatório informar a classificação")]
        [Display(Name = "Classificação")]
        public Classificacoes Classificacao { get; set; }

        public int Duracao { get; set; }

        [NotMapped]
        [Display(Name = "Horas")]
        public int? DuracaoHoras { get; set; }

        [NotMapped]
        [Display(Name = "Minutos")]
        public int? DuracaoMinutos { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a sinopse do filme")]
        [Display(Name = "Sinopse")]
        [StringLength(1000, ErrorMessage = "A sinopse não pode exceder 1000 caracteres")]
        public string Sinopse { get; set; }

        [Display(Name = "URL do Pôster")]
        [StringLength(500)]
        public string? UrlPoster { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Obrigatório informar se está em cartaz o filme.")]
        [Display(Name = "Em cartaz")]
        public bool emCartaz { get; set; }

        public ICollection<Sessao> Sessoes { get; set; } = new List<Sessao>();

        public string GetDuracaoFormatada()
        {
            int horas = Duracao / 60;
            int minutos = Duracao % 60;
            return $"{horas}h {minutos}m";
        }
    }
}
