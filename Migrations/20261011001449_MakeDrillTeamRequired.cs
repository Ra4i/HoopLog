using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoopLog.Migrations
{
    /// <inheritdoc />
    public partial class MakeDrillTeamRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drills_Teams_TeamId",
                table: "Drills");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Drills",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Drills_Teams_TeamId",
                table: "Drills",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drills_Teams_TeamId",
                table: "Drills");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Drills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Drills_Teams_TeamId",
                table: "Drills",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
