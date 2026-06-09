using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCanvas.Web.Migrations
{
    /// <inheritdoc />
    public partial class ApplyTypeToTableStrategy2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dislike_AspNetUsers_ApplicationUserId",
                table: "Dislike");

            migrationBuilder.DropForeignKey(
                name: "FK_Dislike_Posts_PostId1",
                table: "Dislike");

            migrationBuilder.DropForeignKey(
                name: "FK_Dislike_Reactions_Id",
                table: "Dislike");

            migrationBuilder.DropForeignKey(
                name: "FK_EmojiReaction_AspNetUsers_ApplicationUserId",
                table: "EmojiReaction");

            migrationBuilder.DropForeignKey(
                name: "FK_EmojiReaction_Reactions_Id",
                table: "EmojiReaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_AspNetUsers_ApplicationUserId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Posts_PostId2",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Reactions_Id",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_AspNetUsers_UserId",
                table: "Reactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_Posts_PostId",
                table: "Reactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reactions",
                table: "Reactions");

            migrationBuilder.DropIndex(
                name: "IX_Reactions_PostId",
                table: "Reactions");

            migrationBuilder.DropIndex(
                name: "IX_Reactions_UserId_PostId_Type",
                table: "Reactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Like",
                table: "Like");

            migrationBuilder.DropIndex(
                name: "IX_Like_ApplicationUserId",
                table: "Like");

            migrationBuilder.DropIndex(
                name: "IX_Like_PostId2",
                table: "Like");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmojiReaction",
                table: "EmojiReaction");

            migrationBuilder.DropIndex(
                name: "IX_EmojiReaction_ApplicationUserId",
                table: "EmojiReaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dislike",
                table: "Dislike");

            migrationBuilder.DropIndex(
                name: "IX_Dislike_ApplicationUserId",
                table: "Dislike");

            migrationBuilder.DropIndex(
                name: "IX_Dislike_PostId1",
                table: "Dislike");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Like");

            migrationBuilder.DropColumn(
                name: "PostId2",
                table: "Like");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "EmojiReaction");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Dislike");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "Dislike");

            migrationBuilder.RenameTable(
                name: "Reactions",
                newName: "Reaction");

            migrationBuilder.RenameTable(
                name: "Like",
                newName: "Likes");

            migrationBuilder.RenameTable(
                name: "EmojiReaction",
                newName: "EmojiReactions");

            migrationBuilder.RenameTable(
                name: "Dislike",
                newName: "Dislikes");

            migrationBuilder.AddColumn<string>(
                name: "PostId1",
                table: "Reaction",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "Likes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Likes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "EmojiReactions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "Dislikes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Dislikes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reaction",
                table: "Reaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmojiReactions",
                table: "EmojiReactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dislikes",
                table: "Dislikes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_PostId1",
                table: "Reaction",
                column: "PostId1");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_PostId",
                table: "Likes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmojiReactions_UserId",
                table: "EmojiReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dislikes_PostId",
                table: "Dislikes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Dislikes_UserId",
                table: "Dislikes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dislikes_AspNetUsers_UserId",
                table: "Dislikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dislikes_Posts_PostId",
                table: "Dislikes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dislikes_Reaction_Id",
                table: "Dislikes",
                column: "Id",
                principalTable: "Reaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmojiReactions_AspNetUsers_UserId",
                table: "EmojiReactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmojiReactions_Reaction_Id",
                table: "EmojiReactions",
                column: "Id",
                principalTable: "Reaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Posts_PostId",
                table: "Likes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Reaction_Id",
                table: "Likes",
                column: "Id",
                principalTable: "Reaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_Posts_PostId1",
                table: "Reaction",
                column: "PostId1",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dislikes_AspNetUsers_UserId",
                table: "Dislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Dislikes_Posts_PostId",
                table: "Dislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Dislikes_Reaction_Id",
                table: "Dislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_EmojiReactions_AspNetUsers_UserId",
                table: "EmojiReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmojiReactions_Reaction_Id",
                table: "EmojiReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Posts_PostId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Reaction_Id",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_Posts_PostId1",
                table: "Reaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reaction",
                table: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_Reaction_PostId1",
                table: "Reaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_PostId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId",
                table: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmojiReactions",
                table: "EmojiReactions");

            migrationBuilder.DropIndex(
                name: "IX_EmojiReactions_UserId",
                table: "EmojiReactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dislikes",
                table: "Dislikes");

            migrationBuilder.DropIndex(
                name: "IX_Dislikes_PostId",
                table: "Dislikes");

            migrationBuilder.DropIndex(
                name: "IX_Dislikes_UserId",
                table: "Dislikes");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "Reaction");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmojiReactions");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Dislikes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Dislikes");

            migrationBuilder.RenameTable(
                name: "Reaction",
                newName: "Reactions");

            migrationBuilder.RenameTable(
                name: "Likes",
                newName: "Like");

            migrationBuilder.RenameTable(
                name: "EmojiReactions",
                newName: "EmojiReaction");

            migrationBuilder.RenameTable(
                name: "Dislikes",
                newName: "Dislike");

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "Reactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Reactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Like",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostId2",
                table: "Like",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "EmojiReaction",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Dislike",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostId1",
                table: "Dislike",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reactions",
                table: "Reactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Like",
                table: "Like",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmojiReaction",
                table: "EmojiReaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dislike",
                table: "Dislike",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_PostId",
                table: "Reactions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId_PostId_Type",
                table: "Reactions",
                columns: new[] { "UserId", "PostId", "Type" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [PostId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Like_ApplicationUserId",
                table: "Like",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Like_PostId2",
                table: "Like",
                column: "PostId2");

            migrationBuilder.CreateIndex(
                name: "IX_EmojiReaction_ApplicationUserId",
                table: "EmojiReaction",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dislike_ApplicationUserId",
                table: "Dislike",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dislike_PostId1",
                table: "Dislike",
                column: "PostId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Dislike_AspNetUsers_ApplicationUserId",
                table: "Dislike",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dislike_Posts_PostId1",
                table: "Dislike",
                column: "PostId1",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dislike_Reactions_Id",
                table: "Dislike",
                column: "Id",
                principalTable: "Reactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmojiReaction_AspNetUsers_ApplicationUserId",
                table: "EmojiReaction",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmojiReaction_Reactions_Id",
                table: "EmojiReaction",
                column: "Id",
                principalTable: "Reactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Like_AspNetUsers_ApplicationUserId",
                table: "Like",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Posts_PostId2",
                table: "Like",
                column: "PostId2",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Reactions_Id",
                table: "Like",
                column: "Id",
                principalTable: "Reactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
