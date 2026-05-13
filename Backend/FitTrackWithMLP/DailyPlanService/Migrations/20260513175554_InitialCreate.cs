using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyPlanService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyPlans",
                columns: table => new
                {
                    DailyPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPlans", x => x.DailyPlanId);
                });

            migrationBuilder.CreateTable(
                name: "PlannedMeals",
                columns: table => new
                {
                    PlannedMealId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MealId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DailyPlanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedMeals", x => x.PlannedMealId);
                    table.ForeignKey(
                        name: "FK_PlannedMeals_DailyPlans_DailyPlanId",
                        column: x => x.DailyPlanId,
                        principalTable: "DailyPlans",
                        principalColumn: "DailyPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlannedMealIngredients",
                columns: table => new
                {
                    PlannedIngredientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    AmountG = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Protein = table.Column<double>(type: "float", nullable: false),
                    Fat = table.Column<double>(type: "float", nullable: false),
                    Carbs = table.Column<double>(type: "float", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    PlannedMealId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedMealIngredients", x => x.PlannedIngredientId);
                    table.ForeignKey(
                        name: "FK_PlannedMealIngredients_PlannedMeals_PlannedMealId",
                        column: x => x.PlannedMealId,
                        principalTable: "PlannedMeals",
                        principalColumn: "PlannedMealId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlannedMealIngredients_PlannedMealId",
                table: "PlannedMealIngredients",
                column: "PlannedMealId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedMeals_DailyPlanId",
                table: "PlannedMeals",
                column: "DailyPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlannedMealIngredients");

            migrationBuilder.DropTable(
                name: "PlannedMeals");

            migrationBuilder.DropTable(
                name: "DailyPlans");
        }
    }
}
