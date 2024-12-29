using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoHubRefactor.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransferItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "TransferStatus",
                table: "Transfers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "Transfers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ItemId",
                table: "TransferItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferFrom",
                table: "Transfers",
                column: "TransferFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferTo",
                table: "Transfers",
                column: "TransferTo");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Locations_TransferFrom",
                table: "Transfers",
                column: "TransferFrom",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Locations_TransferTo",
                table: "Transfers",
                column: "TransferTo",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Locations_TransferFrom",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Locations_TransferTo",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_TransferFrom",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_TransferTo",
                table: "Transfers");

            migrationBuilder.AlterColumn<string>(
                name: "TransferStatus",
                table: "Transfers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "Transfers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemId",
                table: "TransferItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

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
    }
}
