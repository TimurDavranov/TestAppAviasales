using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class dbinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "references");

            migrationBuilder.EnsureSchema(
                name: "externals");

            migrationBuilder.EnsureSchema(
                name: "aviasales");

            migrationBuilder.CreateTable(
                name: "amadeus_tickets",
                schema: "externals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirportCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumberOfSeats = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amadeus_tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                schema: "aviasales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                schema: "references",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumericCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISOCode2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISOCode3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "amadeus_ticket_destinations",
                schema: "externals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AirportCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AmadeusTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amadeus_ticket_destinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_amadeus_ticket_destinations_amadeus_tickets_AmadeusTicketId",
                        column: x => x.AmadeusTicketId,
                        principalSchema: "externals",
                        principalTable: "amadeus_tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                schema: "aviasales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contracts_bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "aviasales",
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "states",
                schema: "references",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_states", x => x.Id);
                    table.ForeignKey(
                        name: "FK_states_countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "references",
                        principalTable: "countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cities",
                schema: "references",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cities_states_StateId",
                        column: x => x.StateId,
                        principalSchema: "references",
                        principalTable: "states",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "airports",
                schema: "references",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_airports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_airports_cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "references",
                        principalTable: "cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_airports_CityId",
                schema: "references",
                table: "airports",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_amadeus_ticket_destinations_AmadeusTicketId",
                schema: "externals",
                table: "amadeus_ticket_destinations",
                column: "AmadeusTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_cities_StateId",
                schema: "references",
                table: "cities",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_BookingId",
                schema: "aviasales",
                table: "contracts",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_states_CountryId",
                schema: "references",
                table: "states",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "airports",
                schema: "references");

            migrationBuilder.DropTable(
                name: "amadeus_ticket_destinations",
                schema: "externals");

            migrationBuilder.DropTable(
                name: "contracts",
                schema: "aviasales");

            migrationBuilder.DropTable(
                name: "cities",
                schema: "references");

            migrationBuilder.DropTable(
                name: "amadeus_tickets",
                schema: "externals");

            migrationBuilder.DropTable(
                name: "bookings",
                schema: "aviasales");

            migrationBuilder.DropTable(
                name: "states",
                schema: "references");

            migrationBuilder.DropTable(
                name: "countries",
                schema: "references");
        }
    }
}
