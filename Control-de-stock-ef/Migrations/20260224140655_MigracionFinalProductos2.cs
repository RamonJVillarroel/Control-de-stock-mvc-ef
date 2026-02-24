using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Control_de_stock_ef.Migrations
{
    /// <inheritdoc />
    public partial class MigracionFinalProductos2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransaccionStock_Productos_ProductoId",
                table: "TransaccionStock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransaccionStock",
                table: "TransaccionStock");

            migrationBuilder.RenameTable(
                name: "TransaccionStock",
                newName: "TransaccionesStock");

            migrationBuilder.RenameIndex(
                name: "IX_TransaccionStock_ProductoId",
                table: "TransaccionesStock",
                newName: "IX_TransaccionesStock_ProductoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransaccionesStock",
                table: "TransaccionesStock",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransaccionesStock_Productos_ProductoId",
                table: "TransaccionesStock",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransaccionesStock_Productos_ProductoId",
                table: "TransaccionesStock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransaccionesStock",
                table: "TransaccionesStock");

            migrationBuilder.RenameTable(
                name: "TransaccionesStock",
                newName: "TransaccionStock");

            migrationBuilder.RenameIndex(
                name: "IX_TransaccionesStock_ProductoId",
                table: "TransaccionStock",
                newName: "IX_TransaccionStock_ProductoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransaccionStock",
                table: "TransaccionStock",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransaccionStock_Productos_ProductoId",
                table: "TransaccionStock",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
