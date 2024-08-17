using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace City_Easter_Eggs.Migrations
{
    /// <inheritdoc />
    public partial class ImageAdding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "POIs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "POIs");
        }
    }
}
