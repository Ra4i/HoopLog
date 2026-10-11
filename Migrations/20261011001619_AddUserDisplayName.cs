using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoopLog.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Existing users have no name yet: use the part of their email before '@'
            // so rosters never show a full email address.
            migrationBuilder.Sql(@"
UPDATE [AspNetUsers]
SET [DisplayName] = LEFT(
    CASE WHEN CHARINDEX('@', COALESCE([Email], [UserName], '')) > 1
         THEN LEFT(COALESCE([Email], [UserName]), CHARINDEX('@', COALESCE([Email], [UserName])) - 1)
         ELSE COALESCE([Email], [UserName], 'Player')
    END, 50)
WHERE [DisplayName] = '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");
        }
    }
}
