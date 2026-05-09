using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementService.Migrations
{
    /// <inheritdoc />
    public partial class UserDetailsGoalType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoalType",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoalType",
                table: "UserDetails");
        }
    }
}
