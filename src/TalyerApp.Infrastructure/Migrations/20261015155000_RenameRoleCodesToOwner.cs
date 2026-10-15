using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalyerApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameRoleCodesToOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE "Roles" SET "Code" = 'platform_owner', "RoleName" = 'Platform Owner' WHERE "Code" = 'platform_admin';
                UPDATE "Roles" SET "Code" = 'tenant_owner', "RoleName" = 'Tenant Owner' WHERE "Code" = 'tenant_admin';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE "Roles" SET "Code" = 'platform_admin', "RoleName" = 'Platform Administrator' WHERE "Code" = 'platform_owner';
                UPDATE "Roles" SET "Code" = 'tenant_admin', "RoleName" = 'Tenant Administrator' WHERE "Code" = 'tenant_owner';
                """);
        }
    }
}
