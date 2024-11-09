using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Projects.Migrations
{
    /// <inheritdoc />
    public partial class usersinfoaddedtoprojects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAvatarUrl",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Projects",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAvatarUrl",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Projects");
        }
    }
}
