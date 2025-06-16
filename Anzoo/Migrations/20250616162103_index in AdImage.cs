using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anzoo.Migrations
{
    /// <inheritdoc />
    public partial class indexinAdImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderIndex",
                table: "AdImages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderIndex",
                table: "AdImages");
        }
    }
}
