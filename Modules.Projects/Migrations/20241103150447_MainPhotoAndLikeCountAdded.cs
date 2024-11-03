using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Projects.Migrations
{
    /// <inheritdoc />
    public partial class MainPhotoAndLikeCountAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "Photos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "Photos");
        }
    }
}
