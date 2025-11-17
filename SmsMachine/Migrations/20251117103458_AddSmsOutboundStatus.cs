using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmsMachine.Migrations
{
    /// <inheritdoc />
    public partial class AddSmsOutboundStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Sms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sms");
        }
    }
}
