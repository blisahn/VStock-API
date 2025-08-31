using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VBorsa_API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_UpdateSymbolForNonLoginMarketStream2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVisibleFromNonLogin",
                table: "Symbols",
                newName: "IsVisibleForNonLogin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVisibleForNonLogin",
                table: "Symbols",
                newName: "IsVisibleFromNonLogin");
        }
    }
}
