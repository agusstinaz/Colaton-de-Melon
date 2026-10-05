using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColatóndeMelón.Migrations
{
    /// <inheritdoc />
    public partial class CorregirRelacionPedidoDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_Pedidos_PedidoIdPedido",
                table: "PedidoDetalles");

            migrationBuilder.DropIndex(
                name: "IX_PedidoDetalles_PedidoIdPedido",
                table: "PedidoDetalles");

            migrationBuilder.DropColumn(
                name: "PedidoIdPedido",
                table: "PedidoDetalles");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalles_IdPedido",
                table: "PedidoDetalles",
                column: "IdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_Pedidos_IdPedido",
                table: "PedidoDetalles",
                column: "IdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_Pedidos_IdPedido",
                table: "PedidoDetalles");

            migrationBuilder.DropIndex(
                name: "IX_PedidoDetalles_IdPedido",
                table: "PedidoDetalles");

            migrationBuilder.AddColumn<int>(
                name: "PedidoIdPedido",
                table: "PedidoDetalles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalles_PedidoIdPedido",
                table: "PedidoDetalles",
                column: "PedidoIdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_Pedidos_PedidoIdPedido",
                table: "PedidoDetalles",
                column: "PedidoIdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
