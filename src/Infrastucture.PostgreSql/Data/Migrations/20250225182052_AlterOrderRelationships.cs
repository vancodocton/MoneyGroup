using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyGroup.Infrastucture.PostgreSql.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterOrderRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderConsumers_Orders_OrderId",
                table: "OrderConsumers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderConsumers_Users_ConsumerId",
                table: "OrderConsumers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_IssuerId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderConsumers_Orders_OrderId",
                table: "OrderConsumers",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderConsumers_Orders_OrderId",
                table: "OrderConsumers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderConsumers_Users_ConsumerId",
                table: "OrderConsumers");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_IssuerId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderConsumers_Orders_OrderId",
                table: "OrderConsumers",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderConsumers_Users_ConsumerId",
                table: "OrderConsumers",
                column: "ConsumerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_IssuerId",
                table: "Orders",
                column: "IssuerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
