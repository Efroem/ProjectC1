using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoHubRefactor.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransfers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Warehouses_TransferFrom",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Warehouses_TransferTo",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_TransferFrom",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_TransferTo",
                table: "Transfers");

            migrationBuilder.AddColumn<int>(
                name: "TransferId1",
                table: "TransferItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_TransferId1",
                table: "TransferItems",
                column: "TransferId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferItems_Transfers_TransferId1",
                table: "TransferItems",
                column: "TransferId1",
                principalTable: "Transfers",
                principalColumn: "TransferId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferItems_Transfers_TransferId1",
                table: "TransferItems");

            migrationBuilder.DropIndex(
                name: "IX_TransferItems_TransferId1",
                table: "TransferItems");

            migrationBuilder.DropColumn(
                name: "TransferId1",
                table: "TransferItems");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferFrom",
                table: "Transfers",
                column: "TransferFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferTo",
                table: "Transfers",
                column: "TransferTo");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Warehouses_TransferFrom",
                table: "Transfers",
                column: "TransferFrom",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Warehouses_TransferTo",
                table: "Transfers",
                column: "TransferTo",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
