using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImproveNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MessageTitle",
                table: "UserNotifications",
                newName: "TemplateKey");

            migrationBuilder.RenameColumn(
                name: "MessageBody",
                table: "UserNotifications",
                newName: "PayloadJson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TemplateKey",
                table: "UserNotifications",
                newName: "MessageTitle");

            migrationBuilder.RenameColumn(
                name: "PayloadJson",
                table: "UserNotifications",
                newName: "MessageBody");
        }
    }
}
