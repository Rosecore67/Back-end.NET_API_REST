using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P7CreateRestApi.Migrations
{
    public partial class DatabaseCorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RevisionDate",
                table: "Bids",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevisionDate",
                table: "Bids");
        }
    }
}
