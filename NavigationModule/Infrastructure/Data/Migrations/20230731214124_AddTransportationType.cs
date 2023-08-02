using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NavigationModule.Migrations
{
    /// <inheritdoc />
    public partial class AddTransportationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Journeys",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TransportationType",
                table: "Journeys",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TransportationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportationTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_UserId",
                table: "Journeys",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_AspNetUsers_UserId",
                table: "Journeys",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_AspNetUsers_UserId",
                table: "Journeys");

            migrationBuilder.DropTable(
                name: "TransportationTypes");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_UserId",
                table: "Journeys");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Journeys",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransportationType",
                table: "Journeys",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Journeys",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_UserId1",
                table: "Journeys",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_AspNetUsers_UserId1",
                table: "Journeys",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
