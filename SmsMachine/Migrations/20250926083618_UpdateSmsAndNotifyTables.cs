using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmsMachine.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSmsAndNotifyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Notify",
                newName: "TextReport");

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "Sms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SmsIndex",
                table: "Notify",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Notify",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "Sms");

            migrationBuilder.DropColumn(
                name: "SmsIndex",
                table: "Notify");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notify");

            migrationBuilder.RenameColumn(
                name: "TextReport",
                table: "Notify",
                newName: "Text");
        }
    }
}
