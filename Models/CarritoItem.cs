namespace ColatonDeMelon.Models
{
    public class CarritoItem
    {
        public int IdProducto { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public decimal Precio { get; set; }

        public string Imagen { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public decimal Subtotal
        {
            get
            {
                return Precio * Cantidad;
            }
        }
    }
}