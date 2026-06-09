using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCanvas.Web.Migrations
{
    /// <inheritdoc />
    public partial class ApplyTypeToTableStrategy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoThumbnail_Post_PhotoId",
                table: "PhotoThumbnail");

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoThumbnail_Post_PostId",
                table: "PhotoThumbnail");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUsers_ApplicationUserId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUsers_AuthorId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_GalleryId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_PostId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_PostId1",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_AspNetUsers_ApplicationUserId",
                table: "Reaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_AspNetUsers_Dislike_ApplicationUserId",
                table: "Reaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_AspNetUsers_EmojiReaction_ApplicationUserId",
                table: "Reaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_AspNetUsers_UserId",
                table: "Reaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_Post_PostId",
                table: "Reaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_Post_PostId1",
                table: "Reaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_Post_UserId",
                table: "Reaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reaction",
                table: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_Reaction_ApplicationUserId",
                table: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_Reaction_Dislike_ApplicationUserId",
                table: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_Reaction_EmojiReaction_ApplicationUserId",
                table: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_Reaction_PostId1",
                table: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_Reaction_UserId",
                table: "Reaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_ApplicationUserId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_AuthorId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_GalleryId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_PostId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_PostId1",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhotoThumbnail",
                table: "PhotoThumbnail");

            migrationBuilder.DropIndex(
                name: "IX_PhotoThumbnail_PhotoId",
                table: "PhotoThumbnail");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Reaction");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Reaction");

            migrationBuilder.DropColumn(
                name: "Dislike_ApplicationUserId",
                table: "Reaction");

            migrationBuilder.DropColumn(
                name: "EmojiReaction_ApplicationUserId",
                table: "Reaction");

            migrationBuilder.DropColumn(
                name: "EmojiValue",
                table: "Reaction");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "Reaction");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "GalleryId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Gallery_UserTags",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "OriginalFilename",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "UserTags",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "PhotoThumbnail");

            migrationBuilder.RenameTable(
                name: "Reaction",
                newName: "Reactions");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "PhotoThumbnail",
                newName: "PhotoThumbnails");

            migrationBuilder.RenameIndex(
                name: "IX_Reaction_PostId",
                table: "Reactions",
                newName: "IX_Reactions_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_UserId",
                table: "Posts",
                newName: "IX_Posts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PhotoThumbnail_PostId",
                table: "PhotoThumbnails",
                newName: "IX_PhotoThumbnails_PostId");

            migrationBuilder.AddColumn<string>(
                name: "OriginalPhotoId",
                table: "PhotoThumbnails",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reactions",
                table: "Reactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhotoThumbnails",
                table: "PhotoThumbnails",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PostId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "CommentToPostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Posts_Id",
                        column: x => x.Id,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId1",
                        column: x => x.PostId1,
                        principalTable: "Posts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Dislike",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PostId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dislike", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dislike_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dislike_Posts_PostId1",
                        column: x => x.PostId1,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dislike_Reactions_Id",
                        column: x => x.Id,
                        principalTable: "Reactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmojiReaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmojiValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmojiReaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmojiReaction_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmojiReaction_Reactions_Id",
                        column: x => x.Id,
                        principalTable: "Reactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Galleries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Galleries_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Galleries_Posts_Id",
                        column: x => x.Id,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Like",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PostId2 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Like", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Like_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Like_Posts_PostId2",
                        column: x => x.PostId2,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Like_Reactions_Id",
                        column: x => x.Id,
                        principalTable: "Reactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalFilename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserTags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GalleryId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Photos_Galleries_GalleryId",
                        column: x => x.GalleryId,
                        principalTable: "Galleries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Photos_Posts_Id",
                        column: x => x.Id,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId_PostId_Type",
                table: "Reactions",
                columns: new[] { "UserId", "PostId", "Type" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [PostId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoThumbnails_OriginalPhotoId",
                table: "PhotoThumbnails",
                column: "OriginalPhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "Comments",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId1",
                table: "Comments",
                column: "PostId1");

            migrationBuilder.CreateIndex(
                name: "IX_Dislike_ApplicationUserId",
                table: "Dislike",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dislike_PostId1",
                table: "Dislike",
                column: "PostId1");

            migrationBuilder.CreateIndex(
                name: "IX_EmojiReaction_ApplicationUserId",
                table: "EmojiReaction",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_ApplicationUserId",
                table: "Galleries",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Like_ApplicationUserId",
                table: "Like",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Like_PostId2",
                table: "Like",
                column: "PostId2");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_ApplicationUserId",
                table: "Photos",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_GalleryId",
                table: "Photos",
                column: "GalleryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoThumbnails_Photos_OriginalPhotoId",
                table: "PhotoThumbnails",
                column: "OriginalPhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoThumbnails_Photos_PostId",
                table: "PhotoThumbnails",
                column: "PostId",
                principalTable: "Photos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reactions_AspNetUsers_UserId",
                table: "Reactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reactions_Posts_PostId",
                table: "Reactions",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoThumbnails_Photos_OriginalPhotoId",
                table: "PhotoThumbnails");

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoThumbnails_Photos_PostId",
                table: "PhotoThumbnails");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_UserId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_AspNetUsers_UserId",
                table: "Reactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_Posts_PostId",
                table: "Reactions");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Dislike");

            migrationBuilder.DropTable(
                name: "EmojiReaction");

            migrationBuilder.DropTable(
                name: "Like");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Galleries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reactions",
                table: "Reactions");

            migrationBuilder.DropIndex(
                name: "IX_Reactions_UserId_PostId_Type",
                table: "Reactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhotoThumbnails",
                table: "PhotoThumbnails");

            migrationBuilder.DropIndex(
                name: "IX_PhotoThumbnails_OriginalPhotoId",
                table: "PhotoThumbnails");

            migrationBuilder.DropColumn(
                name: "OriginalPhotoId",
                table: "PhotoThumbnails");

            migrationBuilder.RenameTable(
                name: "Reactions",
                newName: "Reaction");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Post");

            migrationBuilder.RenameTable(
                name: "PhotoThumbnails",
                newName: "PhotoThumbnail");

            migrationBuilder.RenameIndex(
                name: "IX_Reactions_PostId",
                table: "Reaction",
                newName: "IX_Reaction_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserId",
                table: "Post",
                newName: "IX_Post_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PhotoThumbnails_PostId",
                table: "PhotoThumbnail",
                newName: "IX_PhotoThumbnail_PostId");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Reaction",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Reaction",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Dislike_ApplicationUserId",
                table: "Reaction",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmojiReaction_ApplicationUserId",
                table: "Reaction",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmojiValue",
                table: "Reaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostId1",
                table: "Reaction",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Post",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Post",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Post",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GalleryId",
                table: "Post",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gallery_UserTags",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalFilename",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "Post",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostId1",
                table: "Post",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Post",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserTags",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoId",
                table: "PhotoThumbnail",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reaction",
                table: "Reaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhotoThumbnail",
                table: "PhotoThumbnail",
                column: "Id");

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
                name: "IX_Reaction_PostId1",
                table: "Reaction",
                column: "PostId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_UserId",
                table: "Reaction",
                column: "UserId");

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
                name: "IX_PhotoThumbnail_PhotoId",
                table: "PhotoThumbnail",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoThumbnail_Post_PhotoId",
                table: "PhotoThumbnail",
                column: "PhotoId",
                principalTable: "Post",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoThumbnail_Post_PostId",
                table: "PhotoThumbnail",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AspNetUsers_ApplicationUserId",
                table: "Post",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AspNetUsers_AuthorId",
                table: "Post",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_GalleryId",
                table: "Post",
                column: "GalleryId",
                principalTable: "Post",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_PostId",
                table: "Post",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_PostId1",
                table: "Post",
                column: "PostId1",
                principalTable: "Post",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_AspNetUsers_ApplicationUserId",
                table: "Reaction",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_AspNetUsers_Dislike_ApplicationUserId",
                table: "Reaction",
                column: "Dislike_ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_AspNetUsers_EmojiReaction_ApplicationUserId",
                table: "Reaction",
                column: "EmojiReaction_ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_AspNetUsers_UserId",
                table: "Reaction",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_Post_PostId",
                table: "Reaction",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_Post_PostId1",
                table: "Reaction",
                column: "PostId1",
                principalTable: "Post",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_Post_UserId",
                table: "Reaction",
                column: "UserId",
                principalTable: "Post",
                principalColumn: "Id");
        }
    }
}
