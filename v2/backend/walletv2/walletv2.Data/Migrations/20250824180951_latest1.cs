using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace walletv2.Data.Migrations;

/// <inheritdoc />
public partial class latest1 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_IncomeExpenses_IncomeExpenseTypeId",
            table: "IncomeExpenses",
            column: "IncomeExpenseTypeId");

        migrationBuilder.CreateIndex(
            name: "IX_Cashflows_CashflowDocumentId",
            table: "Cashflows",
            column: "CashflowDocumentId");

        migrationBuilder.AddForeignKey(
            name: "FK_Cashflows_CashflowDocuments_CashflowDocumentId",
            table: "Cashflows",
            column: "CashflowDocumentId",
            principalTable: "CashflowDocuments",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_IncomeExpenses_IncomeExpenseTypes_IncomeExpenseTypeId",
            table: "IncomeExpenses",
            column: "IncomeExpenseTypeId",
            principalTable: "IncomeExpenseTypes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Cashflows_CashflowDocuments_CashflowDocumentId",
            table: "Cashflows");

        migrationBuilder.DropForeignKey(
            name: "FK_IncomeExpenses_IncomeExpenseTypes_IncomeExpenseTypeId",
            table: "IncomeExpenses");

        migrationBuilder.DropIndex(
            name: "IX_IncomeExpenses_IncomeExpenseTypeId",
            table: "IncomeExpenses");

        migrationBuilder.DropIndex(
            name: "IX_Cashflows_CashflowDocumentId",
            table: "Cashflows");
    }
}
