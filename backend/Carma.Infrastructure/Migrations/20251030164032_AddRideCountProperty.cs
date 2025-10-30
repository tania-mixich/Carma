using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRideCountProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RidesCount",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RidesCount",
                table: "Users");
        }
    }
}
