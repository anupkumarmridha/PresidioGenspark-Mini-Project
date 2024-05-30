using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApparelShoppingAppAPI.Migrations
{
    public partial class UpdatedOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isPaid",
                table: "Orders",
                newName: "IsPaid");

            migrationBuilder.AddColumn<bool>(
                name: "IsCanceled",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelivered",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCanceled",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDelivered",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "IsPaid",
                table: "Orders",
                newName: "isPaid");
        }
    }
}
