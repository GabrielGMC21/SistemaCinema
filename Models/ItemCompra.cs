using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Cinema.Models
{
    public class ItemCompra
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 999999, ErrorMessage = "A quantidade deve ser de no mínimo 1")]
        public int Quantidade { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0, 9999999.99, ErrorMessage = "O subtotal deve ser um valor positivo")]
        public decimal Subtotal { get; set; }

        public int IdCompra { get; set; }
        public int IdProduto { get; set; }

        [ForeignKey("IdCompra")]
        public Compra Compra { get; set; }

        [ForeignKey("IdProduto")]
        public Produto Produto { get; set; }
    }
}
