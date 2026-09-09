using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Byd.FileManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemovedStoredFileStatusLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_stored_files_stored_file_statuses_status_id",
                schema: "file_management",
                table: "StoredFiles");

            migrationBuilder.DropTable(
                name: "StoredFileStatuses",
                schema: "file_management");

            migrationBuilder.DropIndex(
                name: "ix_stored_files_status_id",
                schema: "file_management",
                table: "StoredFiles");

            migrationBuilder.DropColumn(
                name: "status_id",
                schema: "file_management",
                table: "StoredFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "status_id",
                schema: "file_management",
                table: "StoredFiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "StoredFileStatuses",
                schema: "file_management",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stored_file_statuses", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_stored_files_status_id",
                schema: "file_management",
                table: "StoredFiles",
                column: "status_id");

            migrationBuilder.AddForeignKey(
                name: "fk_stored_files_stored_file_statuses_status_id",
                schema: "file_management",
                table: "StoredFiles",
                column: "status_id",
                principalSchema: "file_management",
                principalTable: "StoredFileStatuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
