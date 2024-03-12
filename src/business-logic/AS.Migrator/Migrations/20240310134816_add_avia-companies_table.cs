using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AS.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class add_aviacompanies_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresDate",
                schema: "aviasales",
                table: "bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "AviaCompanyId",
                schema: "externals",
                table: "amadeus_tickets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "avia_companies",
                schema: "references",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avia_companies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "avia_companies",
                schema: "references");

            migrationBuilder.DropColumn(
                name: "ExpiresDate",
                schema: "aviasales",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "AviaCompanyId",
                schema: "externals",
                table: "amadeus_tickets");
        }
    }
}
