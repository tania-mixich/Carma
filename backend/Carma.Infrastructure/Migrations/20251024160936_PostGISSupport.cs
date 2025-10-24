using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Carma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PostGISSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DropOffLatitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "DropOffLongitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PickupLatitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PickupLongitude",
                table: "Rides");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<Point>(
                name: "Coordinate",
                table: "Users",
                type: "geography(Point, 4326)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Rides",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Available",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<Point>(
                name: "DropOffCoordinate",
                table: "Rides",
                type: "geography(Point, 4326)",
                nullable: false);

            migrationBuilder.AddColumn<Point>(
                name: "PickupCoordinate",
                table: "Rides",
                type: "geography(Point, 4326)",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "RideRole",
                table: "RideParticipants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "NotAssigned",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Coordinate",
                table: "Users",
                column: "Coordinate")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_Rides_DropOffCoordinate",
                table: "Rides",
                column: "DropOffCoordinate")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Coordinate",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Rides_DropOffCoordinate",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "Coordinate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DropOffCoordinate",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PickupCoordinate",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Users",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Users",
                type: "double precision",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Rides",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Available");

            migrationBuilder.AddColumn<double>(
                name: "DropOffLatitude",
                table: "Rides",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DropOffLongitude",
                table: "Rides",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PickupLatitude",
                table: "Rides",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PickupLongitude",
                table: "Rides",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "RideRole",
                table: "RideParticipants",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "NotAssigned");
        }
    }
}
