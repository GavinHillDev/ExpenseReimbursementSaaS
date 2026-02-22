using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseReimbursmentSaaS.Migrations
{
    /// <inheritdoc />
    public partial class ReportAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Receipt_ExpenseReportId",
                table: "Receipt",
                column: "ExpenseReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_ExpenseReport_ExpenseReportId",
                table: "Receipt",
                column: "ExpenseReportId",
                principalTable: "ExpenseReport",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_ExpenseReport_ExpenseReportId",
                table: "Receipt");

            migrationBuilder.DropIndex(
                name: "IX_Receipt_ExpenseReportId",
                table: "Receipt");
        }
    }
}
