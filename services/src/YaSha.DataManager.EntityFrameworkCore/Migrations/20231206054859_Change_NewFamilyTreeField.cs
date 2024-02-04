using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YaSha.DataManager.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNewFamilyTreeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Show",
                table: "AppNewFamilyTree",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Show",
                table: "AppNewFamilyTree",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }
    }
}
