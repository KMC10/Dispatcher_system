using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DHLManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Drivers_DriverId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_DriverId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "AvailableCapacity",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "DriverId",
                table: "Trips",
                newName: "RemainingCapacity");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Trips",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "RemainingCapacity",
                table: "Trips",
                newName: "DriverId");

            migrationBuilder.AddColumn<int>(
                name: "AvailableCapacity",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DriverId",
                table: "Trips",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Drivers_DriverId",
                table: "Trips",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
