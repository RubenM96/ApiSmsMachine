using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmsMachine.Migrations
{
    /// <inheritdoc />
    public partial class campaignCounters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveredCount",
                table: "CampaignSms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FailedCount",
                table: "CampaignSms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRecipients",
                table: "CampaignSms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredCount",
                table: "CampaignSms");

            migrationBuilder.DropColumn(
                name: "FailedCount",
                table: "CampaignSms");

            migrationBuilder.DropColumn(
                name: "TotalRecipients",
                table: "CampaignSms");
        }
    }
}
