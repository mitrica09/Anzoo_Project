using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anzoo.Migrations
{
    /// <inheritdoc />
    public partial class contactintoad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Ads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Ads",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Ads");
        }
    }
}
