using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyGroup.Infrastucture.SqlServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterOrderAggregate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderConsumers_Orders_OrderId",
                table: "OrderConsumers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderConsumers_Users_ParticipantId",
                table: "OrderConsumers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderConsumers",
                table: "OrderConsumers");

            migrationBuilder.RenameTable(
                name: "OrderConsumers",
                newName: "OrderParticipants");

            migrationBuilder.RenameIndex(
                name: "IX_OrderConsumers_ParticipantId",
                table: "OrderParticipants",
                newName: "IX_OrderParticipants_ParticipantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderParticipants",
                table: "OrderParticipants",
                columns: new[] { "OrderId", "ParticipantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderParticipants_Orders_OrderId",
                table: "OrderParticipants",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderParticipants_Users_ParticipantId",
                table: "OrderParticipants",
                column: "ParticipantId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderParticipants_Orders_OrderId",
                table: "OrderParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderParticipants_Users_ParticipantId",
                table: "OrderParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderParticipants",
                table: "OrderParticipants");

            migrationBuilder.RenameTable(
                name: "OrderParticipants",
                newName: "OrderConsumers");

            migrationBuilder.RenameIndex(
                name: "IX_OrderParticipants_ParticipantId",
                table: "OrderConsumers",
                newName: "IX_OrderConsumers_ParticipantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderConsumers",
                table: "OrderConsumers",
                columns: new[] { "OrderId", "ParticipantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderConsumers_Orders_OrderId",
                table: "OrderConsumers",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderConsumers_Users_ParticipantId",
                table: "OrderConsumers",
                column: "ParticipantId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
