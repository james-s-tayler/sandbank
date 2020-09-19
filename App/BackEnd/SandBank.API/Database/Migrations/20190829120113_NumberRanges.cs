using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Api.Migrations
{
    public partial class NumberRanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NumberRanges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RangeType = table.Column<string>(maxLength: 25, nullable: false),
                    Prefix = table.Column<string>(maxLength: 10, nullable: false),
                    RangeStart = table.Column<int>(nullable: false, defaultValue: 1),
                    RangeEnd = table.Column<int>(nullable: false, defaultValue: 999999999),
                    LastValue = table.Column<int>(nullable: false, defaultValue: 0),
                    ShadowId = table.Column<Guid>(nullable: false),
                    xmin = table.Column<uint>(type: "xid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberRanges", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumberRanges");
        }
    }
}
