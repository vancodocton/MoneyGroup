using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyGroup.Infrastucture.PostgreSql.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterOrderTruncate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderParticipants_Orders_OrderId",
                table: "OrderParticipants");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderParticipants_Orders_OrderId",
                table: "OrderParticipants",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderParticipants_Orders_OrderId",
                table: "OrderParticipants");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderParticipants_Orders_OrderId",
                table: "OrderParticipants",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
