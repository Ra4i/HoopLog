using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoopLog.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamOwnedDrills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Drills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Drills",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsArchived", "TeamId" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsArchived", "TeamId" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsArchived", "TeamId" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "IsArchived", "TeamId" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "IsArchived", "TeamId" },
                values: new object[] { false, null });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "IsArchived", "TeamId" },
                values: new object[] { false, null });

            migrationBuilder.CreateIndex(
                name: "IX_Drills_TeamId",
                table: "Drills",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drills_Teams_TeamId",
                table: "Drills",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drills_Teams_TeamId",
                table: "Drills");

            migrationBuilder.DropIndex(
                name: "IX_Drills_TeamId",
                table: "Drills");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Drills");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Drills");
        }
    }
}
