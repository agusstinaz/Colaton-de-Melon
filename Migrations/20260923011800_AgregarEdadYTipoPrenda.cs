using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColatóndeMelón.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEdadYTipoPrenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================================
            // PRODUCTO → EDAD
            // =========================================

            migrationBuilder.AddColumn<int>(
                name: "IdEdad",
                table: "Productos",
                type: "int",
                nullable: true);


            // =========================================
            // PRODUCTO → TIPO DE PRENDA
            // =========================================

            migrationBuilder.AddColumn<int>(
                name: "IdTipoPrenda",
                table: "Productos",
                type: "int",
                nullable: true);


            // =========================================
            // ÍNDICE EDAD
            // =========================================

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdEdad",
                table: "Productos",
                column: "IdEdad");


            // =========================================
            // ÍNDICE TIPO DE PRENDA
            // =========================================

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdTipoPrenda",
                table: "Productos",
                column: "IdTipoPrenda");


            // =========================================
            // RELACIÓN PRODUCTO → EDAD
            // =========================================

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Edades_IdEdad",
                table: "Productos",
                column: "IdEdad",
                principalTable: "Edades",
                principalColumn: "IdEdad",
                onDelete: ReferentialAction.Restrict);


            // =========================================
            // RELACIÓN PRODUCTO → TIPO DE PRENDA
            // =========================================

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_TiposPrenda_IdTipoPrenda",
                table: "Productos",
                column: "IdTipoPrenda",
                principalTable: "TiposPrenda",
                principalColumn: "IdTipoPrenda",
                onDelete: ReferentialAction.Restrict);
        }


        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Edades_IdEdad",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_TiposPrenda_IdTipoPrenda",
                table: "Productos");


            migrationBuilder.DropIndex(
                name: "IX_Productos_IdEdad",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_IdTipoPrenda",
                table: "Productos");


            migrationBuilder.DropColumn(
                name: "IdEdad",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "IdTipoPrenda",
                table: "Productos");
        }
    }
}