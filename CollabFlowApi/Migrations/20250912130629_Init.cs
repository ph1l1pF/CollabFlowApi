using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabFlowApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "collaborations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Deadline_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deadline_SendNotification = table.Column<bool>(type: "boolean", nullable: false),
                    Deadline_NotifyDaysBefore = table.Column<int>(type: "integer", nullable: true),
                    Fee_Amount = table.Column<double>(type: "double precision", nullable: false),
                    Fee_Currency = table.Column<string>(type: "text", nullable: false),
                    Partner_CompanyName = table.Column<string>(type: "text", nullable: false),
                    Partner_Industry = table.Column<string>(type: "text", nullable: false),
                    Partner_Name = table.Column<string>(type: "text", nullable: false),
                    Partner_Email = table.Column<string>(type: "text", nullable: false),
                    Partner_Phone = table.Column<string>(type: "text", nullable: false),
                    Partner_CustomerNumber = table.Column<string>(type: "text", nullable: false),
                    Script_Content = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collaborations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AppleSub = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_collaborations_Id_UserId",
                table: "collaborations",
                columns: new[] { "Id", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_AppleSub",
                table: "users",
                column: "AppleSub",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "collaborations");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
