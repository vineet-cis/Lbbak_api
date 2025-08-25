using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lbbak_api.Migrations
{
    /// <inheritdoc />
    public partial class Edit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionalOffers");

            migrationBuilder.DropTable(
                name: "OfferCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfferCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromotionalOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSingleUsePerUser = table.Column<bool>(type: "bit", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxUsageCount = table.Column<int>(type: "int", nullable: true),
                    MediaId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionalOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionalOffers_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionalOffers_OfferCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "OfferCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionalOffers_CategoryId",
                table: "PromotionalOffers",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionalOffers_CityId",
                table: "PromotionalOffers",
                column: "CityId");
        }
    }
}
