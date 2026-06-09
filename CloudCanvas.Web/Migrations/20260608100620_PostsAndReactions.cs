using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCanvas.Web.Migrations
{
    /// <inheritdoc />
    public partial class PostsAndReactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProPicUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PublishedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UnpublishedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ContentLength = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommentsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PostId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gallery_UserTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalFilename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GalleryId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Post_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Post_Post_GalleryId",
                        column: x => x.GalleryId,
                        principalTable: "Post",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Post_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Post_Post_PostId1",
                        column: x => x.PostId1,
                        principalTable: "Post",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PhotoThumbnail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OriginalImageURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoThumbnail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoThumbnail_Post_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Post",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhotoThumbnail_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Dislike_ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmojiValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmojiReaction_ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PostId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reaction_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reaction_AspNetUsers_Dislike_ApplicationUserId",
                        column: x => x.Dislike_ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reaction_AspNetUsers_EmojiReaction_ApplicationUserId",
                        column: x => x.EmojiReaction_ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reaction_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reaction_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reaction_Post_PostId1",
                        column: x => x.PostId1,
                        principalTable: "Post",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reaction_Post_UserId",
                        column: x => x.UserId,
                        principalTable: "Post",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoThumbnail_PhotoId",
                table: "PhotoThumbnail",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoThumbnail_PostId",
                table: "PhotoThumbnail",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ApplicationUserId",
                table: "Post",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_AuthorId",
                table: "Post",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_GalleryId",
                table: "Post",
                column: "GalleryId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostId",
                table: "Post",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostId1",
                table: "Post",
                column: "PostId1");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserId",
                table: "Post",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_ApplicationUserId",
                table: "Reaction",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_Dislike_ApplicationUserId",
                table: "Reaction",
                column: "Dislike_ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_EmojiReaction_ApplicationUserId",
                table: "Reaction",
                column: "EmojiReaction_ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_PostId",
                table: "Reaction",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_PostId1",
                table: "Reaction",
                column: "PostId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_UserId",
                table: "Reaction",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoThumbnail");

            migrationBuilder.DropTable(
                name: "Reaction");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.AlterColumn<string>(
                name: "ProPicUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
