using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelExpress.Migrations
{
    /// <inheritdoc />
    public partial class agmi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Hotels_HotelId",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hotels",
                table: "Hotels");

            migrationBuilder.RenameTable(
                name: "Hotels",
                newName: "Hotel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hotel",
                table: "Hotel",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Hotel_HotelId",
                table: "Rooms",
                column: "HotelId",
                principalTable: "Hotel",
                principalColumn: "HotelId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Hotel_HotelId",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hotel",
                table: "Hotel");

            migrationBuilder.RenameTable(
                name: "Hotel",
                newName: "Hotels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hotels",
                table: "Hotels",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Hotels_HotelId",
                table: "Rooms",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "HotelId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
