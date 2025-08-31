using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VBorsa_API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class bug_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId_SymbolId_ExecutedAt",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Symbols_Source_Code",
                table: "Symbols");

            migrationBuilder.DropIndex(
                name: "IX_Holdings_UserId_SymbolId",
                table: "Holdings");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "Symbols",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Symbols_Code",
                table: "Symbols",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Holdings_UserId",
                table: "Holdings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Symbols_Code",
                table: "Symbols");

            migrationBuilder.DropIndex(
                name: "IX_Holdings_UserId",
                table: "Holdings");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "Symbols",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_SymbolId_ExecutedAt",
                table: "Transactions",
                columns: new[] { "UserId", "SymbolId", "ExecutedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Symbols_Source_Code",
                table: "Symbols",
                columns: new[] { "Source", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_Holdings_UserId_SymbolId",
                table: "Holdings",
                columns: new[] { "UserId", "SymbolId" });
        }
    }
}
