using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LBManage.Migrations
{
    /// <inheritdoc />
    public partial class addSuggestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SuggestedBooks",
                columns: table => new
                {
                    SuggestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Course = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    SuggestedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuggestedBooks", x => x.SuggestionID);
                    table.ForeignKey(
                        name: "FK_SuggestedBooks_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SuggestedBooks_StudentID",
                table: "SuggestedBooks",
                column: "StudentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SuggestedBooks");
        }
    }
}
