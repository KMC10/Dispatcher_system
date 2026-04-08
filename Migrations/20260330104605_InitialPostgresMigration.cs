using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DHLManagementSystem.Migrations
{
    public partial class PostgresMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Vehicles table
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Vehicles",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "PlateNumber",
                table: "Vehicles",
                type: "text",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Vehicles",
                type: "text",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Vehicles",
                type: "integer",
                nullable: false);

            // Trips table
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Trips",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "TransportRouteId",
                table: "Trips",
                type: "integer",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "RemainingCapacity",
                table: "Trips",
                type: "integer",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Trips",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                @"ALTER TABLE ""Trips""
                  ALTER COLUMN ""DepartureTime"" TYPE timestamp with time zone
                  USING ""DepartureTime""::timestamp with time zone;");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DepartureTime",
                table: "Trips",
                type: "timestamp with time zone",
                nullable: false);

            // Shipments table
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Shipments",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Origin",
                table: "Shipments",
                type: "text",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "Shipments",
                type: "text",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Shipments",
                type: "text",
                nullable: false);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "Shipments",
                type: "double precision",
                nullable: false);

            migrationBuilder.Sql(
                @"ALTER TABLE ""Shipments""
                  ALTER COLUMN ""DeliveryDeadline"" TYPE timestamp with time zone
                  USING ""DeliveryDeadline""::timestamp with time zone;");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDeadline",
                table: "Shipments",
                type: "timestamp with time zone",
                nullable: false);

            // ShipmentAssignments table
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ShipmentAssignments",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ShipmentId",
                table: "ShipmentAssignments",
                type: "integer",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "TripId",
                table: "ShipmentAssignments",
                type: "integer",
                nullable: false);

            migrationBuilder.Sql(
                @"ALTER TABLE ""ShipmentAssignments""
                  ALTER COLUMN ""AssignedAt"" TYPE timestamp with time zone
                  USING ""AssignedAt""::timestamp with time zone;");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignedAt",
                table: "ShipmentAssignments",
                type: "timestamp with time zone",
                nullable: false);

            // Routes table
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Routes",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Origin",
                table: "Routes",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "Routes",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DistanceKm",
                table: "Routes",
                type: "double precision",
                nullable: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "BasePrice",
                table: "Routes",
                type: "numeric",
                nullable: false);

            // Drivers table
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Drivers",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Drivers",
                type: "text",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Drivers",
                type: "text",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Drivers",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "Drivers",
                type: "boolean",
                nullable: false);

            // Identity tables (AspNetUsers, Roles, Claims, etc.)
            // Similar pattern: convert TEXT columns, booleans, integers, and timestamp with time zone
            // Example for AspNetUsers
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetUsers",
                type: "text",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            // Repeat similar conversions for all other Identity tables as in your example
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse all column changes, restore old types (TEXT, INTEGER, REAL, etc.)
            // Include removing identity annotations
            // Example:
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Vehicles",
                type: "INTEGER",
                nullable: false)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "PlateNumber",
                table: "Vehicles",
                type: "TEXT",
                nullable: false);

            // Repeat reverse conversions for all tables...
        }
    }
}