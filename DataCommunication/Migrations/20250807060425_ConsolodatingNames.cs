using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lbbak_api.Migrations
{
    /// <inheritdoc />
    public partial class ConsolodatingNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add 'Name' column first — it must exist before you update it!
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true
            );

            // 2. Update Name from IndividualUsers
            migrationBuilder.Sql(@"
                UPDATE U
                SET Name = COALESCE(I.FirstName, '') + ' ' + COALESCE(I.LastName, '')
                FROM Users U
                INNER JOIN IndividualUsers I ON U.Id = I.Id
            ");

            // 3. Update Name from CompanyUsers
            migrationBuilder.Sql(@"
                UPDATE U
                SET Name = C.CompanyName
                FROM Users U
                INNER JOIN CompanyUsers C ON U.Id = C.Id
            ");

            // 4. Update Name from DesignerUsers
            migrationBuilder.Sql(@"
                UPDATE U
                SET Name = D.FullName
                FROM Users U
                INNER JOIN DesignerUsers D ON U.Id = D.Id
            ");

            // 5. Drop old fields AFTER data migration
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "IndividualUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "IndividualUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "DesignerUsers");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "CompanyUsers");

            // 6. Modify other columns as needed
            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "IndividualUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "IndividualUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "DesignerUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "CompanyUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
