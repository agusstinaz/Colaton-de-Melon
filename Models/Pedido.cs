using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ColatonDeMelon.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Localidad { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string CodigoPostal { get; set; } = string.Empty;

        [StringLength(500)]
        public string Observaciones { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        public DateTime FechaPedido { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = string.Empty;

        public ICollection<PedidoDetalle> Detalles { get; set; }
            = new List<PedidoDetalle>();
    }
}