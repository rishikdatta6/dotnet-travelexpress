using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelExpress.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHotelForeignKeyFromRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Hotel_HotelId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Hotel");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hotel",
                columns: table => new
                {
                    HotelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotel", x => x.HotelId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Hotel_HotelId",
                table: "Rooms",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "HotelId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
