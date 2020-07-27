using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesApi.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Insert into AspNetRoles (Id, [name], [NormalizedName]) values ('d397a4f5-47d5-4062-9ae9-b5b5d6964376', 'Admin', 'Admin')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete AspNetRoles where id = 'd397a4f5-47d5-4062-9ae9-b5b5d6964376'");
        }
    }
}
