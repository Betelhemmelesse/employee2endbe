using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeHierarchy.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeNameToPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "Positions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "Positions");
        }
    }
}
