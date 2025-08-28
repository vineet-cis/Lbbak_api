using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lbbak_api.Migrations
{
    /// <inheritdoc />
    public partial class AdminPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_AdminRoles_AdminRoleId",
                table: "Admins");

            migrationBuilder.AlterColumn<int>(
                name: "AdminRoleId",
                table: "Admins",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Countries",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Permissions",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_AdminRoles_AdminRoleId",
                table: "Admins",
                column: "AdminRoleId",
                principalTable: "AdminRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_AdminRoles_AdminRoleId",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "Countries",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "Admins");

            migrationBuilder.AlterColumn<int>(
                name: "AdminRoleId",
                table: "Admins",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_AdminRoles_AdminRoleId",
                table: "Admins",
                column: "AdminRoleId",
                principalTable: "AdminRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
