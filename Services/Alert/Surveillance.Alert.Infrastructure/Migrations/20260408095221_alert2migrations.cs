using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surveillance.Alert.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class alert2migrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Error",
                schema: "alert",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeadLetter",
                schema: "alert",
                table: "OutboxMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                schema: "alert",
                table: "OutboxMessages",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error",
                schema: "alert",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "IsDeadLetter",
                schema: "alert",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                schema: "alert",
                table: "OutboxMessages");
        }
    }
}
