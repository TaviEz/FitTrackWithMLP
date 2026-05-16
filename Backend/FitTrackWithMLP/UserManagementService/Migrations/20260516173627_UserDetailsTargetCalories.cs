using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementService.Migrations
{
    /// <inheritdoc />
    public partial class UserDetailsTargetCalories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetCalories",
                table: "UserDetails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetCalories",
                table: "UserDetails");
        }
    }
}
