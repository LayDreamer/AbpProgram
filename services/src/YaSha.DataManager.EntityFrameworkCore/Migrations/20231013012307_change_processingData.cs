using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YaSha.DataManager.Migrations
{
    /// <inheritdoc />
    public partial class changeprocessingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppProcessingDatas",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppProcessingDatas",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppProcessingDatas",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "AppProcessingDatas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppProcessingDatas",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppProcessingDatas",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppProcessingDatas");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppProcessingDatas");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppProcessingDatas");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "AppProcessingDatas");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppProcessingDatas");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppProcessingDatas");
        }
    }
}
