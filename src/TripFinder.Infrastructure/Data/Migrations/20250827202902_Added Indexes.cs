using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripFinder.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PickupLocation",
                table: "Trips",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DropoffLocation",
                table: "Trips",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DistanceKm",
                table: "Trips",
                column: "DistanceKm");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DropoffLocation",
                table: "Trips",
                column: "DropoffLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DurationMinutes",
                table: "Trips",
                column: "DurationMinutes");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_PickupLocation",
                table: "Trips",
                column: "PickupLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_RequestDate",
                table: "Trips",
                column: "RequestDate");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Status",
                table: "Trips",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Type",
                table: "Trips",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trips_DistanceKm",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_DropoffLocation",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_DurationMinutes",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_PickupLocation",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_RequestDate",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_Status",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_Type",
                table: "Trips");

            migrationBuilder.AlterColumn<string>(
                name: "PickupLocation",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "DropoffLocation",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
