using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestApi.Migrations.Contracts
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ContractorFirstName = table.Column<string>(type: "text", nullable: false),
                    ContractorLastName = table.Column<string>(type: "text", nullable: false),
                    ContractorEmail = table.Column<string>(type: "text", nullable: false),
                    ContractorPhoneNumber = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");
        }
    }
}
