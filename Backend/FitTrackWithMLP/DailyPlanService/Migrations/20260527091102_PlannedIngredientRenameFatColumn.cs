using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyPlanService.Migrations
{
    /// <inheritdoc />
    public partial class PlannedIngredientRenameFatColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fat",
                table: "PlannedMealIngredients",
                newName: "Fats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fats",
                table: "PlannedMealIngredients",
                newName: "Fat");
        }
    }
}
