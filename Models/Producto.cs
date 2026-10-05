using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColatonDeMelon.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

    [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public string? Imagen { get; set; }


        // ==========================================
        // CATEGORÍA POR EDAD
        // ==========================================

        public int? IdEdad { get; set; }

        [ForeignKey(nameof(IdEdad))]
        public Edad? Edad { get; set; }


        // ==========================================
        // CATEGORÍA POR TIPO DE PRENDA
        // ==========================================

        public int? IdTipoPrenda { get; set; }

        [ForeignKey(nameof(IdTipoPrenda))]
        public TipoPrenda? TipoPrenda { get; set; }


        // ==========================================
        // CATEGORÍA ANTIGUA
        // Se mantiene por compatibilidad
        // ==========================================

        public int IdCategoria { get; set; }

        [ForeignKey(nameof(IdCategoria))]
        public Categoria? Categoria { get; set; }
    }

}
