using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Byd.FileManagement.Migrations
{
    /// <inheritdoc />
    public partial class RenameAbpColumnsToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TenantId",
                schema: "file_management",
                table: "StoredFiles",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "LastModifierId",
                schema: "file_management",
                table: "StoredFiles",
                newName: "last_modifier_id");

            migrationBuilder.RenameColumn(
                name: "LastModificationTime",
                schema: "file_management",
                table: "StoredFiles",
                newName: "last_modification_time");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                schema: "file_management",
                table: "StoredFiles",
                newName: "extra_properties");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                schema: "file_management",
                table: "StoredFiles",
                newName: "creator_id");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "file_management",
                table: "StoredFiles",
                newName: "creation_time");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                schema: "file_management",
                table: "StoredFiles",
                newName: "concurrency_stamp");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                schema: "file_management",
                table: "FileCategories",
                newName: "creator_id");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "file_management",
                table: "FileCategories",
                newName: "creation_time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tenant_id",
                schema: "file_management",
                table: "StoredFiles",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "last_modifier_id",
                schema: "file_management",
                table: "StoredFiles",
                newName: "LastModifierId");

            migrationBuilder.RenameColumn(
                name: "last_modification_time",
                schema: "file_management",
                table: "StoredFiles",
                newName: "LastModificationTime");

            migrationBuilder.RenameColumn(
                name: "extra_properties",
                schema: "file_management",
                table: "StoredFiles",
                newName: "ExtraProperties");

            migrationBuilder.RenameColumn(
                name: "creator_id",
                schema: "file_management",
                table: "StoredFiles",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "creation_time",
                schema: "file_management",
                table: "StoredFiles",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "concurrency_stamp",
                schema: "file_management",
                table: "StoredFiles",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameColumn(
                name: "creator_id",
                schema: "file_management",
                table: "FileCategories",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "creation_time",
                schema: "file_management",
                table: "FileCategories",
                newName: "CreationTime");
        }
    }
}
