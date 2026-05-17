using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyPlanService.Migrations
{
    /// <inheritdoc />
    public partial class DailyPlanTargetDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "TargetDate",
                table: "DailyPlans",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlans_UserId_TargetDate",
                table: "DailyPlans",
                columns: new[] { "UserId", "TargetDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyPlans_UserId_TargetDate",
                table: "DailyPlans");

            migrationBuilder.DropColumn(
                name: "TargetDate",
                table: "DailyPlans");
        }
    }
}
