using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShainingOpt.Migrations
{
    /// <inheritdoc />
    public partial class DeleteIsActiveFromCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Companies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Companies",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
