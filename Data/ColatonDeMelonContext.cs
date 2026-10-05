using ColatonDeMelon.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ColatonDeMelon.Data
{
    public class ColatonDeMelonContext : IdentityDbContext
    {
        public ColatonDeMelonContext(
            DbContextOptions<ColatonDeMelonContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Edad> Edades { get; set; }

        public DbSet<TipoPrenda> TiposPrenda { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }

        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =========================================
            // PRODUCTO → EDAD
            // =========================================

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Edad)
                .WithMany(e => e.Productos)
                .HasForeignKey(p => p.IdEdad)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================
            // PRODUCTO → TIPO DE PRENDA
            // =========================================

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.TipoPrenda)
                .WithMany(t => t.Productos)
                .HasForeignKey(p => p.IdTipoPrenda)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}