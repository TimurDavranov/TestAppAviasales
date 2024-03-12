using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class sdaokpdsa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_skyscanner_ticket_destinations_skyscanner_tickets_AmadeusTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations");

            migrationBuilder.RenameColumn(
                name: "AmadeusTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                newName: "SkyscannerTicketId");

            migrationBuilder.RenameIndex(
                name: "IX_skyscanner_ticket_destinations_AmadeusTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                newName: "IX_skyscanner_ticket_destinations_SkyscannerTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_skyscanner_ticket_destinations_skyscanner_tickets_SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                column: "SkyscannerTicketId",
                principalSchema: "externals",
                principalTable: "skyscanner_tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_skyscanner_ticket_destinations_skyscanner_tickets_SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations");

            migrationBuilder.RenameColumn(
                name: "SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                newName: "AmadeusTicketId");

            migrationBuilder.RenameIndex(
                name: "IX_skyscanner_ticket_destinations_SkyscannerTicketId",
                schema: "externals",
                table: "skyscanner_ticket_destinations",
                newName: "IX_skyscanner_ticket_destinations_AmadeusTicketId");

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
    }
}
