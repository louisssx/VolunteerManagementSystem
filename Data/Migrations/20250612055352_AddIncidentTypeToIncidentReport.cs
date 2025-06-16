using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentTypeToIncidentReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "IncidentReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "IncidentReports");
        }
    }
}
