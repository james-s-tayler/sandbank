using Microsoft.EntityFrameworkCore.Migrations;

namespace Endpoints.Migrations
{
    public partial class ChangeDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RangeEnd",
                table: "NumberRanges",
                nullable: false,
                defaultValue: 999999,
                oldClrType: typeof(int),
                oldDefaultValue: 999999999);

            migrationBuilder.AlterColumn<string>(
                name: "AccountType",
                table: "Accounts",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RangeEnd",
                table: "NumberRanges",
                nullable: false,
                defaultValue: 999999999,
                oldClrType: typeof(int),
                oldDefaultValue: 999999);

            migrationBuilder.AlterColumn<string>(
                name: "AccountType",
                table: "Accounts",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
