using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NotificationUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Rides_RideId",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "RideId",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Rides_RideId",
                table: "Notifications",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Rides_RideId",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "RideId",
                table: "Notifications",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Rides_RideId",
                table: "Notifications",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id");
        }
    }
}
