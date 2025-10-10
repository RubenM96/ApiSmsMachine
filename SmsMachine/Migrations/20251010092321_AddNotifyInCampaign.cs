using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmsMachine.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifyInCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CampaignNotify",
                table: "CampaignSms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampaignNotify",
                table: "CampaignSms");
        }
    }
}
