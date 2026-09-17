using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoopLog.Migrations
{
    /// <inheritdoc />
    public partial class SeedDrillMetricTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                column: "MetricType",
                value: "Count");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 4,
                column: "MetricType",
                value: "Duration");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 5,
                column: "MetricType",
                value: "Count");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 6,
                column: "MetricType",
                value: "Count");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                column: "MetricType",
                value: "MakesAttempts");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 4,
                column: "MetricType",
                value: "MakesAttempts");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 5,
                column: "MetricType",
                value: "MakesAttempts");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 6,
                column: "MetricType",
                value: "MakesAttempts");
        }
    }
}
