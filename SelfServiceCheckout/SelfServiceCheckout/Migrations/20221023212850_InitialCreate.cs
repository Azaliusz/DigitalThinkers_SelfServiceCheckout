using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfServiceCheckout.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoneyDenominations",
                columns: table => new
                {
                    Currency = table.Column<int>(type: "INTEGER", nullable: false),
                    Denomination = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyDenominations", x => new { x.Currency, x.Denomination });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoneyDenominations");
        }
    }
}
