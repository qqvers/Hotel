using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotel_backend.API.Migrations
{
    public partial class removedRedunantCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Owners_OwnerId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_OwnerId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Customers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_OwnerId",
                table: "Customers",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Owners_OwnerId",
                table: "Customers",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id");
        }
    }
}
