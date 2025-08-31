using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VBorsa_API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_2_updated_entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Symbols_Source_Code",
                table: "Symbols");

            migrationBuilder.AddColumn<string>(
                name: "AssetClass",
                table: "Symbols",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Symbols_Source_Code",
                table: "Symbols",
                columns: new[] { "Source", "Code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Symbols_Source_Code",
                table: "Symbols");

            migrationBuilder.DropColumn(
                name: "AssetClass",
                table: "Symbols");

            migrationBuilder.CreateIndex(
                name: "IX_Symbols_Source_Code",
                table: "Symbols",
                columns: new[] { "Source", "Code" },
                unique: true);
        }
    }
}
