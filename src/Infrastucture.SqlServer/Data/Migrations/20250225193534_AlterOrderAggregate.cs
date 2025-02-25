using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyGroup.Infrastucture.SqlServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterOrderAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderConsumers_Users_ConsumerId",
                table: "OrderConsumers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_IssuerId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "IssuerId",
                table: "Orders",
                newName: "BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_IssuerId",
                table: "Orders",
                newName: "IX_Orders_BuyerId");

            migrationBuilder.RenameColumn(
                name: "ConsumerId",
                table: "OrderConsumers",
                newName: "ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderConsumers_ConsumerId",
                table: "OrderConsumers",
                newName: "IX_OrderConsumers_ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderConsumers_Users_ParticipantId",
                table: "OrderConsumers",
                column: "ParticipantId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_BuyerId",
                table: "Orders",
                column: "BuyerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderConsumers_Users_ParticipantId",
                table: "OrderConsumers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_BuyerId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "BuyerId",
                table: "Orders",
                newName: "IssuerId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_BuyerId",
                table: "Orders",
                newName: "IX_Orders_IssuerId");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "OrderConsumers",
                newName: "ConsumerId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderConsumers_ParticipantId",
                table: "OrderConsumers",
                newName: "IX_OrderConsumers_ConsumerId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderConsumers_Users_ConsumerId",
                table: "OrderConsumers",
                column: "ConsumerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_IssuerId",
                table: "Orders",
                column: "IssuerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
