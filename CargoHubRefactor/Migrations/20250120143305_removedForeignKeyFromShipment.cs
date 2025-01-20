using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoHubRefactor.Migrations
{
    /// <inheritdoc />
    public partial class removedForeignKeyFromShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Warehouses_SourceId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_SourceId",
                table: "Shipments");

            migrationBuilder.AlterColumn<int>(
                name: "SourceId",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SourceId",
                table: "Orders",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_SourceId",
                table: "Shipments",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Warehouses_SourceId",
                table: "Shipments",
                column: "SourceId",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
