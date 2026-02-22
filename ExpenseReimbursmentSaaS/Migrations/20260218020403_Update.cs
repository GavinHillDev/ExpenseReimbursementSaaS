using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseReimbursmentSaaS.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseReport_Employee_UploaderId",
                table: "ExpenseReport");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseReport_UploaderId",
                table: "ExpenseReport");

            migrationBuilder.RenameColumn(
                name: "ExpentReportId",
                table: "Receipt",
                newName: "ExpenseReportId");

            migrationBuilder.AlterColumn<int>(
                name: "managerId",
                table: "ExpenseReport",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FinanceId",
                table: "ExpenseReport",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpenseReportId",
                table: "Receipt",
                newName: "ExpentReportId");

            migrationBuilder.AlterColumn<int>(
                name: "managerId",
                table: "ExpenseReport",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FinanceId",
                table: "ExpenseReport",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseReport_UploaderId",
                table: "ExpenseReport",
                column: "UploaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseReport_Employee_UploaderId",
                table: "ExpenseReport",
                column: "UploaderId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
