using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColatonDeMelon.Models
{
    public class PedidoDetalle
    {
        [Key]
        public int IdPedidoDetalle { get; set; }

        [ForeignKey(nameof(Pedido))]
        public int IdPedido { get; set; }

        public int IdProducto { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreProducto { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        public Pedido Pedido { get; set; } = null!;
    }
}