using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class iodasjidsa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_skyscanner_ticket_destinations_amadeus_tickets_AmadeusTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations");

            migrationBuilder.DropForeignKey(
                name: "FK_skyscanner_ticket_destinations_skyscanner_tickets_SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations");

            migrationBuilder.DropIndex(
                name: "IX_skyscanner_ticket_destinations_SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations");

            migrationBuilder.DropColumn(
                name: "SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations");

            migrationBuilder.AddForeignKey(
                name: "FK_skyscanner_ticket_destinations_skyscanner_tickets_AmadeusTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                column: "AmadeusTicketId",
                principalSchema: "externals",
                principalTable: "skyscanner_tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_skyscanner_ticket_destinations_skyscanner_tickets_AmadeusTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations");

            migrationBuilder.AddColumn<Guid>(
                name: "SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_skyscanner_ticket_destinations_SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                column: "SkyscannerTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_skyscanner_ticket_destinations_amadeus_tickets_AmadeusTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                column: "AmadeusTicketId",
                principalSchema: "externals",
                principalTable: "amadeus_tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_skyscanner_ticket_destinations_skyscanner_tickets_SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                column: "SkyscannerTicketId",
                principalSchema: "externals",
                principalTable: "skyscanner_tickets",
                principalColumn: "Id");
        }
    }
}
