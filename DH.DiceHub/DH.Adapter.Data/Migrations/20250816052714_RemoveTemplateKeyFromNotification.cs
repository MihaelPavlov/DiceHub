using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTemplateKeyFromNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateKey",
                table: "UserNotifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TemplateKey",
                table: "UserNotifications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
