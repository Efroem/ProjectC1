using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoHubRefactor.Migrations
{
    /// <inheritdoc />
    public partial class AddedSoftDeleteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Warehouses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Transfers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "TransferItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Suppliers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Shipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "ShipmentItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Locations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "ItemTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "ItemLines",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "ItemGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Inventories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "TransferItems");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "ShipmentItems");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "ItemLines");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "ItemGroups");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Clients");
        }
    }
}
