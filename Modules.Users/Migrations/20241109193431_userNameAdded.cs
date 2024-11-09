using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Users.Migrations
{
    /// <inheritdoc />
    public partial class userNameAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "UsersProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "UsersProfiles");
        }
    }
}
