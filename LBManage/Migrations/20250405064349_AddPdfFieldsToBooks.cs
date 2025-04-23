using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBManage.Migrations
{
    /// <inheritdoc />
    public partial class AddPdfFieldsToBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfFileName",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfLink",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfFileName",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PdfLink",
                table: "Books");
        }
    }
}
