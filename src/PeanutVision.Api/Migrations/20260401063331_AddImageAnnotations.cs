using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeanutVision.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddImageAnnotations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CapturedImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ThumbnailPath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Format = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    Notes = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapturedImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapturedImages_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapturedImages_CapturedAt",
                table: "CapturedImages",
                column: "CapturedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CapturedImages_SessionId",
                table: "CapturedImages",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CreatedAt",
                table: "Sessions",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapturedImages");

            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}
