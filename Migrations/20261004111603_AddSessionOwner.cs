using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoopLog.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "TrainingSessions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_OwnerId",
                table: "TrainingSessions",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingSessions_AspNetUsers_OwnerId",
                table: "TrainingSessions",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingSessions_AspNetUsers_OwnerId",
                table: "TrainingSessions");

            migrationBuilder.DropIndex(
                name: "IX_TrainingSessions_OwnerId",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "TrainingSessions");
        }
    }
}
