using Microsoft.EntityFrameworkCore.Migrations;

namespace Endpoints.Migrations
{
    public partial class UpdateTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionCategory",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TransactionClassification",
                table: "Transactions",
                newName: "TransactionType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Transactions",
                newName: "TransactionClassification");

            migrationBuilder.AddColumn<string>(
                name: "TransactionCategory",
                table: "Transactions",
                maxLength: 25,
                nullable: true);
        }
    }
}
