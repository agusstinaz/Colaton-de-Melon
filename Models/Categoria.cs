using System.ComponentModel.DataAnnotations;

namespace ColatonDeMelon.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}