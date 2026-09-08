using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Byd.FileManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "file_management");

            migrationBuilder.CreateTable(
                name: "FileCategories",
                schema: "file_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    code = table.Column<int>(type: "integer", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "StoredFileStatuses",
                schema: "file_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    code = table.Column<int>(type: "integer", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stored_file_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "StoredFiles",
                schema: "file_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<Guid>(type: "uuid", nullable: false),
                    module_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    blob_name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    original_file_name = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stored_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_stored_files_file_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "file_management",
                        principalTable: "FileCategories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_stored_files_stored_file_statuses_status_id",
                        column: x => x.status_id,
                        principalSchema: "file_management",
                        principalTable: "StoredFileStatuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileCategories_Name",
                schema: "file_management",
                table: "FileCategories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_ExternalId_CategoryId",
                schema: "file_management",
                table: "StoredFiles",
                columns: new[] { "external_id", "category_id" });

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_TenantId",
                schema: "file_management",
                table: "StoredFiles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "ix_stored_files_category_id",
                schema: "file_management",
                table: "StoredFiles",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_stored_files_status_id",
                schema: "file_management",
                table: "StoredFiles",
                column: "status_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoredFiles",
                schema: "file_management");

            migrationBuilder.DropTable(
                name: "FileCategories",
                schema: "file_management");

            migrationBuilder.DropTable(
                name: "StoredFileStatuses",
                schema: "file_management");
        }
    }
}
