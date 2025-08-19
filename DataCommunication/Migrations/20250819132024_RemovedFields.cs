using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lbbak_api.Migrations
{
    /// <inheritdoc />
    public partial class RemovedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeactivatedAt",
                table: "IndividualUsers");

            migrationBuilder.DropColumn(
                name: "IsDeactivated",
                table: "IndividualUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "IndividualUsers");

            migrationBuilder.DropColumn(
                name: "DesignSpeciality",
                table: "DesignerUsers");

            migrationBuilder.DropColumn(
                name: "PortfolioLink",
                table: "DesignerUsers");

            migrationBuilder.DropColumn(
                name: "DeactivatedAt",
                table: "CompanyUsers");

            migrationBuilder.DropColumn(
                name: "IsDeactivated",
                table: "CompanyUsers");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "CompanyUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAt",
                table: "IndividualUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeactivated",
                table: "IndividualUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "IndividualUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesignSpeciality",
                table: "DesignerUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortfolioLink",
                table: "DesignerUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAt",
                table: "CompanyUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeactivated",
                table: "CompanyUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "CompanyUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
