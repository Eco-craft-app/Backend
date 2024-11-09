using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Users.Migrations
{
    /// <inheritdoc />
    public partial class PrimaryKeyRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UsersProfiles",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UsersProfiles",
                newName: "Id");
        }
    }
}
