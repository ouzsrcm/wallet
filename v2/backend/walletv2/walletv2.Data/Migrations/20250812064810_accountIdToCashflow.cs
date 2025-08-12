using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace walletv2.Data.Migrations
{
    /// <inheritdoc />
    public partial class accountIdToCashflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Cashflows",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Cashflows_AccountId",
                table: "Cashflows",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cashflows_Accounts_AccountId",
                table: "Cashflows",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cashflows_Accounts_AccountId",
                table: "Cashflows");

            migrationBuilder.DropIndex(
                name: "IX_Cashflows_AccountId",
                table: "Cashflows");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Cashflows");
        }
    }
}
