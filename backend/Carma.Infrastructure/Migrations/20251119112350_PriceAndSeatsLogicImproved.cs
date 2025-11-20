using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PriceAndSeatsLogicImproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerSeat",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "AvailableSeats",
                table: "Rides",
                newName: "Seats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Seats",
                table: "Rides",
                newName: "AvailableSeats");

            migrationBuilder.AddColumn<double>(
                name: "PricePerSeat",
                table: "Rides",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
