using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCanvas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageTagsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "SinglePhoto_ManyThumbnails",
                table: "PhotoThumbnails");

            migrationBuilder.DropForeignKey(
                name: "SinglePost_ManyReactions",
                table: "Reactions");

            migrationBuilder.DropForeignKey(
                name: "SingleUser_ManyReactions",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "SmartTags",
                table: "Photos");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Posts",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.CreateTable(
                name: "SmartTags",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Confidence = table.Column<double>(type: "float", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartTags", x => x.Id);
                    table.ForeignKey(
                        name: "SinglePhoto_ManySmartTags",
                        column: x => x.PostId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmartTags_PostId_Name",
                table: "SmartTags",
                columns: new[] { "PostId", "Name" },
                unique: true,
                filter: "[PostId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "SinglePhoto_ManyThumbnails",
                table: "PhotoThumbnails",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "SinglePost_ManyReactions",
                table: "Reactions",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "SingleUser_ManyReactions",
                table: "Reactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "SinglePhoto_ManyThumbnails",
                table: "PhotoThumbnails");

            migrationBuilder.DropForeignKey(
                name: "SinglePost_ManyReactions",
                table: "Reactions");

            migrationBuilder.DropForeignKey(
                name: "SingleUser_ManyReactions",
                table: "Reactions");

            migrationBuilder.DropTable(
                name: "SmartTags");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Posts",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<string>(
                name: "SmartTags",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "SinglePhoto_ManyThumbnails",
                table: "PhotoThumbnails",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "SinglePost_ManyReactions",
                table: "Reactions",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "SingleUser_ManyReactions",
                table: "Reactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
