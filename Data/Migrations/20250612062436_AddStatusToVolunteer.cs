using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToVolunteer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Volunteers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Volunteers");
        }
    }
}
