using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace City_Easter_Eggs.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LikesObtained = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsernameNormalized = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "POIs",
                columns: table => new
                {
                    PointId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    TimeStamp = table.Column<long>(type: "bigint", nullable: false),
                    CreatorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POIs", x => x.PointId);
                    table.ForeignKey(
                        name: "FK_POIs_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavouritePoints",
                columns: table => new
                {
                    FavoriteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PointId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouritePoints", x => x.FavoriteId);
                    table.ForeignKey(
                        name: "FK_FavouritePoints_POIs_PointId",
                        column: x => x.PointId,
                        principalTable: "POIs",
                        principalColumn: "PointId");
                    table.ForeignKey(
                        name: "FK_FavouritePoints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "LikedPoints",
                columns: table => new
                {
                    LikedId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PointId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikedPoints", x => x.LikedId);
                    table.ForeignKey(
                        name: "FK_LikedPoints_POIs_PointId",
                        column: x => x.PointId,
                        principalTable: "POIs",
                        principalColumn: "PointId");
                    table.ForeignKey(
                        name: "FK_LikedPoints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavouritePoints_PointId",
                table: "FavouritePoints",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouritePoints_UserId",
                table: "FavouritePoints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LikedPoints_PointId",
                table: "LikedPoints",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_LikedPoints_UserId",
                table: "LikedPoints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_POIs_CreatorUserId",
                table: "POIs",
                column: "CreatorUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouritePoints");

            migrationBuilder.DropTable(
                name: "LikedPoints");

            migrationBuilder.DropTable(
                name: "POIs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
