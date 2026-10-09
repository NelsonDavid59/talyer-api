using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalyerApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantCodeAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Tenants",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Tenants",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Code",
                table: "Tenants",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Type",
                table: "Tenants",
                column: "Type",
                unique: true,
                filter: "\"Type\" = 'PLATFORM'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenants_Code",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Type",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tenants");
        }
    }
}
