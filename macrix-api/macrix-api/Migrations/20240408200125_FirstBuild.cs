using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace macrix_api.Migrations
{
    /// <inheritdoc />
    public partial class FirstBuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    StreetName = table.Column<string>(type: "TEXT", nullable: false),
                    HouseNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ApartmentNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: false),
                    Town = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
