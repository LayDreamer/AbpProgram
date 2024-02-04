using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YaSha.DataManager.Migrations
{
    /// <inheritdoc />
    public partial class StandardAndPolicyAddNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "AppStandardAndPolicyTree",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DispatchFont",
                table: "AppStandardAndPolicyLib",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "LoseDate",
                table: "AppStandardAndPolicyLib",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StandardCategory",
                table: "AppStandardAndPolicyLib",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AppStandardAndPolicyLib",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "AppStandardAndPolicyTree");

            migrationBuilder.DropColumn(
                name: "DispatchFont",
                table: "AppStandardAndPolicyLib");

            migrationBuilder.DropColumn(
                name: "LoseDate",
                table: "AppStandardAndPolicyLib");

            migrationBuilder.DropColumn(
                name: "StandardCategory",
                table: "AppStandardAndPolicyLib");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppStandardAndPolicyLib");
        }
    }
}
