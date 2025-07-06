using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anzoo.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionfieldsintoAd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPromoted",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PromotionExpiresAt",
                table: "Ads",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPromoted",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "PromotionExpiresAt",
                table: "Ads");
        }
    }
}
