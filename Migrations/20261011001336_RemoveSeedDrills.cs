using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HoopLog.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedDrills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Drills",
                columns: new[] { "Id", "Category", "Description", "IsArchived", "MetricType", "Name", "TeamId" },
                values: new object[,]
                {
                    { 1, 0, "Shoot free throws from the line, track makes out of attempts.", false, "MakesAttempts", "Free Throws", null },
                    { 2, 0, "Shoot from five fixed spots around the arc.", false, "MakesAttempts", "Spot-Up Jumpers", null },
                    { 3, 1, "Weave through cones using both hands.", false, "Count", "Cone Dribbling", null },
                    { 4, 2, "Sprint down and back at increasing distances.", false, "Duration", "Suicides", null },
                    { 5, 4, "Lateral slides in a defensive stance.", false, "Count", "Defensive Slides", null },
                    { 6, 3, "Bodyweight strength circuit.", false, "Count", "Core Circuit", null }
                });
        }
    }
}
