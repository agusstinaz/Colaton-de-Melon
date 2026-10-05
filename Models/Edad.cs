using System.ComponentModel.DataAnnotations;

namespace ColatonDeMelon.Models
{
    public class Edad
    {
        [Key]
        public int IdEdad { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Producto> Productos { get; set; }
            = new List<Producto>();
    }
}