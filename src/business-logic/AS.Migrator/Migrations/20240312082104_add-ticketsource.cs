using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class addticketsource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Source",
                schema: "aviasales",
                table: "bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "skyscanner_tickets",
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
                    AviaCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skyscanner_tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "skyscanner_ticket_destinations",
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
                    SkyscannerTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skyscanner_ticket_destinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_skyscanner_ticket_destinations_amadeus_tickets_AmadeusTicketId",
                        column: x => x.AmadeusTicketId,
                        principalSchema: "externals",
                        principalTable: "amadeus_tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_skyscanner_ticket_destinations_skyscanner_tickets_SkyscannerTicketId",
                        column: x => x.SkyscannerTicketId,
                        principalSchema: "externals",
                        principalTable: "skyscanner_tickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_skyscanner_ticket_destinations_AmadeusTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                column: "AmadeusTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_skyscanner_ticket_destinations_SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                column: "SkyscannerTicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "skyscanner_ticket_destinations",
                schema: "externals");

            migrationBuilder.DropTable(
                name: "skyscanner_tickets",
                schema: "externals");

            migrationBuilder.DropColumn(
                name: "Source",
                schema: "aviasales",
                table: "bookings");
        }
    }
}
