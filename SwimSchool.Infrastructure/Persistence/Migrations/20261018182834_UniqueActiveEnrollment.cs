using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwimSchool.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UniqueActiveEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_GroupId",
                table: "Enrollments");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_GroupId",
                table: "Enrollments",
                columns: new[] { "StudentId", "GroupId" },
                unique: true,
                filter: "\"IsActive\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_GroupId",
                table: "Enrollments");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_GroupId",
                table: "Enrollments",
                columns: new[] { "StudentId", "GroupId" },
                unique: true);
        }
    }
}
