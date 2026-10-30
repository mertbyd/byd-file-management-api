using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Byd.FileManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFileCategoryMaxFileSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "max_file_size",
                schema: "file_management",
                table: "FileCategories",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "max_file_size",
                schema: "file_management",
                table: "FileCategories");
        }
    }
}
