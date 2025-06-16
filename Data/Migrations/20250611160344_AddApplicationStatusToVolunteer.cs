using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationStatusToVolunteer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationStatus",
                table: "Volunteers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_VolunteerId",
                table: "TaskAssignments",
                column: "VolunteerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAssignments_Volunteers_VolunteerId",
                table: "TaskAssignments",
                column: "VolunteerId",
                principalTable: "Volunteers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskAssignments_Volunteers_VolunteerId",
                table: "TaskAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TaskAssignments_VolunteerId",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "ApplicationStatus",
                table: "Volunteers");
        }
    }
}
