using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCanvas.Web.Migrations
{
    /// <inheritdoc />
    public partial class AppUserUpdatedInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HousNumber",
                table: "Address",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "Address");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Address",
                newName: "HousNumber");
        }
    }
}
