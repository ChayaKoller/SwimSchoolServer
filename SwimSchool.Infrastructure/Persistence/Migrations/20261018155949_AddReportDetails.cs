using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwimSchool.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReportDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DefaultGroupPrice",
                table: "Users",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 90m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultIndividualPrice",
                table: "Users",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultTreatmentsCount",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 16);

            migrationBuilder.AddColumn<string>(
                name: "ReportSignature",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceCompany",
                table: "Reports",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerTreatment",
                table: "Reports",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 90m);

            migrationBuilder.AddColumn<string>(
                name: "ServiceDescription",
                table: "Reports",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "טיפול רגשי במים");

            migrationBuilder.AddColumn<int>(
                name: "TreatmentType",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultGroupPrice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DefaultIndividualPrice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DefaultTreatmentsCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReportSignature",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InsuranceCompany",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "PricePerTreatment",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ServiceDescription",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "TreatmentType",
                table: "Reports");
        }
    }
}
