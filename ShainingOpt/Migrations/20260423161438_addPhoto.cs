using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShainingOpt.Migrations
{
    /// <inheritdoc />
    public partial class addPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ProductVariant",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ProductVariant");
        }
    }
}
