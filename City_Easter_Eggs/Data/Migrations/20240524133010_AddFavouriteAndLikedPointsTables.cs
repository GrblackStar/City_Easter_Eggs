using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace City_Easter_Eggs.Migrations
{
    /// <inheritdoc />
    public partial class AddFavouriteAndLikedPointsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavouritePoints",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PointId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PointOfInterestPointId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouritePoints", x => new { x.UserId, x.PointId });
                    table.ForeignKey(
                        name: "FK_FavouritePoints_POIs_PointOfInterestPointId",
                        column: x => x.PointOfInterestPointId,
                        principalTable: "POIs",
                        principalColumn: "PointId");
                    table.ForeignKey(
                        name: "FK_FavouritePoints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LikedPoints",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PointId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PointOfInterestPointId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikedPoints", x => new { x.UserId, x.PointId });
                    table.ForeignKey(
                        name: "FK_LikedPoints_POIs_PointOfInterestPointId",
                        column: x => x.PointOfInterestPointId,
                        principalTable: "POIs",
                        principalColumn: "PointId");
                    table.ForeignKey(
                        name: "FK_LikedPoints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavouritePoints_PointOfInterestPointId",
                table: "FavouritePoints",
                column: "PointOfInterestPointId");

            migrationBuilder.CreateIndex(
                name: "IX_LikedPoints_PointOfInterestPointId",
                table: "LikedPoints",
                column: "PointOfInterestPointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouritePoints");

            migrationBuilder.DropTable(
                name: "LikedPoints");
        }
    }
}
