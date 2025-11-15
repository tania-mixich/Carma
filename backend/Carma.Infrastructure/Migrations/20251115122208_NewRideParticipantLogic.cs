using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewRideParticipantLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "RideParticipants");

            migrationBuilder.DropColumn(
                name: "RideRole",
                table: "RideParticipants");

            migrationBuilder.AddColumn<DateTime>(
                name: "LeftAt",
                table: "RideParticipants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "RideParticipants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "RideParticipants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RideParticipants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftAt",
                table: "RideParticipants");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "RideParticipants");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "RideParticipants");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RideParticipants");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "RideParticipants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RideRole",
                table: "RideParticipants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "NotAssigned");
        }
    }
}
