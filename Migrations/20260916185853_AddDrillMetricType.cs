using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoopLog.Migrations
{
    /// <inheritdoc />
    public partial class AddDrillMetricType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetricType",
                table: "Drills",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "MakesAttempts");

            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "DrillResults",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetricType",
                table: "Drills");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "DrillResults");
        }
    }
}
