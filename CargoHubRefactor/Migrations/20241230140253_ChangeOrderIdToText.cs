using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoHubRefactor.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrderIdToText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add a temporary column with the new type
            migrationBuilder.AddColumn<string>(
                name: "OrderIdTemp",
                table: "Shipments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            // Step 2: Copy existing data to the new column, converting integers to strings
            migrationBuilder.Sql("UPDATE Shipments SET OrderIdTemp = CAST(OrderId AS TEXT)");

            // Step 3: Drop the old column
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Shipments");

            // Step 4: Rename the temporary column to the original column name
            migrationBuilder.RenameColumn(
                name: "OrderIdTemp",
                table: "Shipments",
                newName: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the migration (if needed)
            migrationBuilder.AddColumn<int>(
                name: "OrderIdTemp",
                table: "Shipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Shipments SET OrderIdTemp = CAST(OrderId AS INTEGER)");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Shipments");

            migrationBuilder.RenameColumn(
                name: "OrderIdTemp",
                table: "Shipments",
                newName: "OrderId");
        }
    }

}

