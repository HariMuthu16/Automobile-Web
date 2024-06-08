using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopSpeed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PostUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Post");
        }
    }
}
