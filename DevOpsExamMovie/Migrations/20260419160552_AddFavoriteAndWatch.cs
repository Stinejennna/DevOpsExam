using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevOpsExamMovie.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteAndWatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Movies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ToWatch",
                table: "Movies",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "ToWatch",
                table: "Movies");
        }
    }
}
