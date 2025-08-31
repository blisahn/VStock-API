using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VBorsa_API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_updated_contraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Holdings_UserId_SymbolId",
                table: "Holdings");

            migrationBuilder.CreateIndex(
                name: "IX_Holdings_UserId_SymbolId",
                table: "Holdings",
                columns: new[] { "UserId", "SymbolId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Holdings_UserId_SymbolId",
                table: "Holdings");

            migrationBuilder.CreateIndex(
                name: "IX_Holdings_UserId_SymbolId",
                table: "Holdings",
                columns: new[] { "UserId", "SymbolId" },
                unique: true);
        }
    }
}
