using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VBorsa_API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_UpdateSymbolForNonLoginMarketStream : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Symbols_Code",
                table: "Symbols");

            migrationBuilder.AlterColumn<string>(
                name: "AssetClass",
                table: "Symbols",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsVisibleFromNonLogin",
                table: "Symbols",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Symbols_Code_AssetClass",
                table: "Symbols",
                columns: new[] { "Code", "AssetClass" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Symbols_Code_AssetClass",
                table: "Symbols");

            migrationBuilder.DropColumn(
                name: "IsVisibleFromNonLogin",
                table: "Symbols");

            migrationBuilder.AlterColumn<string>(
                name: "AssetClass",
                table: "Symbols",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Symbols_Code",
                table: "Symbols",
                column: "Code");
        }
    }
}
