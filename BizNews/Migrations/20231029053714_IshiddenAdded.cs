using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizNews.Migrations
{
    /// <inheritdoc />
    public partial class IshiddenAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ishidden",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ishidden",
                table: "Articles");
        }
    }
}
