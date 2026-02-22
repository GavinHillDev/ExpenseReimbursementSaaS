using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseReimbursmentSaaS.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_ExpenseReport_expenseReportId",
                table: "Receipt");

            migrationBuilder.DropIndex(
                name: "IX_Receipt_expenseReportId",
                table: "Receipt");

            migrationBuilder.RenameColumn(
                name: "expenseReportId",
                table: "Receipt",
                newName: "UploaderId");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "Receipt",
                newName: "Category");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Receipt",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "FinanceId",
                table: "ExpenseReport",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UploaderId",
                table: "ExpenseReport",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "managerId",
                table: "ExpenseReport",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExpenseCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategory", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseReport_Employee_UploaderId",
                table: "ExpenseReport");

            migrationBuilder.DropTable(
                name: "ExpenseCategory");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseReport_UploaderId",
                table: "ExpenseReport");

            migrationBuilder.DropColumn(
                name: "FinanceId",
                table: "ExpenseReport");

            migrationBuilder.DropColumn(
                name: "UploaderId",
                table: "ExpenseReport");

            migrationBuilder.DropColumn(
                name: "managerId",
                table: "ExpenseReport");

            migrationBuilder.RenameColumn(
                name: "UploaderId",
                table: "Receipt",
                newName: "expenseReportId");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Receipt",
                newName: "ContentType");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "Receipt",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_expenseReportId",
                table: "Receipt",
                column: "expenseReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_ExpenseReport_expenseReportId",
                table: "Receipt",
                column: "expenseReportId",
                principalTable: "ExpenseReport",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
