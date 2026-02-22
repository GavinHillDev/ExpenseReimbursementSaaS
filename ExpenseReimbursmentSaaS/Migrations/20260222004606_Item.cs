using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseReimbursmentSaaS.Migrations
{
    /// <inheritdoc />
    public partial class Item : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "ExpenseItem",
                newName: "UploadDate");

            migrationBuilder.AddColumn<int>(
                name: "UploaderId",
                table: "ExpenseItem",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploaderId",
                table: "ExpenseItem");

            migrationBuilder.RenameColumn(
                name: "UploadDate",
                table: "ExpenseItem",
                newName: "Date");
        }
    }
}
