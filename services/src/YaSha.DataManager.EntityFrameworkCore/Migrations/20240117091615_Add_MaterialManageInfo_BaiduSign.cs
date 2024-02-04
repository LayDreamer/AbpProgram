using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YaSha.DataManager.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialManageInfoBaiduSign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaiduSign",
                table: "AppMaterialManageInfo",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaiduSign",
                table: "AppMaterialManageInfo");
        }
    }
}
