using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zuricos.Folio.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDomainForPortfolioAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Accounts_AccountId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Assets",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Assets",
                newName: "CreatedUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "AssetHistories",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Activities",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Activities",
                newName: "TransferGroupId");

            migrationBuilder.RenameColumn(
                name: "PriceCurrency",
                table: "Activities",
                newName: "BookCurrency");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Activities",
                newName: "OccurredOn");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Accounts",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "Accounts",
                newName: "DisplayCurrency");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Accounts",
                newName: "CreatedUtc");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "Assets",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedUtc",
                table: "Assets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Assets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Assets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume",
                table: "AssetHistories",
                type: "numeric(20,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Open",
                table: "AssetHistories",
                type: "numeric(19,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Low",
                table: "AssetHistories",
                type: "numeric(19,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "High",
                table: "AssetHistories",
                type: "numeric(19,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Close",
                table: "AssetHistories",
                type: "numeric(19,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AdjustedClose",
                table: "AssetHistories",
                type: "numeric(19,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedUtc",
                table: "AssetHistories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedUtc",
                table: "AssetHistories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AssetHistories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Tax",
                table: "Activities",
                type: "numeric(19,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,6)");

            migrationBuilder.AlterColumn<string>(
                name: "FeesCurrency",
                table: "Activities",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "Fees",
                table: "Activities",
                type: "numeric(19,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetId",
                table: "Activities",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Activities",
                type: "numeric(19,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,8)");

            migrationBuilder.AddColumn<decimal>(
                name: "CounterAmount",
                table: "Activities",
                type: "numeric(19,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CounterCurrency",
                table: "Activities",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedUtc",
                table: "Activities",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedUtc",
                table: "Activities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FxRate",
                table: "Activities",
                type: "numeric(18,8)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Activities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "Activities",
                type: "numeric(20,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SettledAmount",
                table: "Activities",
                type: "numeric(19,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "Activities",
                type: "numeric(19,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitPriceCurrency",
                table: "Activities",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Accounts",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedUtc",
                table: "Accounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PortfolioId",
                table: "Accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Institution = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    DisplayCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Portfolios_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Symbol",
                table: "Assets",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_UserId_IsDeleted",
                table: "Assets",
                columns: new[] { "UserId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_PortfolioId",
                table: "Accounts",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId_PortfolioId_IsDeleted",
                table: "Accounts",
                columns: new[] { "UserId", "PortfolioId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_UserId_IsDeleted",
                table: "Portfolios",
                columns: new[] { "UserId", "IsDeleted" });

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Portfolios_PortfolioId",
                table: "Accounts",
                column: "PortfolioId",
                principalTable: "Portfolios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Accounts_AccountId",
                table: "Activities",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Portfolios_PortfolioId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Accounts_AccountId",
                table: "Activities");

            migrationBuilder.DropTable(
                name: "Portfolios");

            migrationBuilder.DropIndex(
                name: "IX_Assets_Symbol",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_UserId_IsDeleted",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_PortfolioId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_UserId_PortfolioId_IsDeleted",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DeletedUtc",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "AssetHistories");

            migrationBuilder.DropColumn(
                name: "DeletedUtc",
                table: "AssetHistories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AssetHistories");

            migrationBuilder.DropColumn(
                name: "CounterAmount",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CounterCurrency",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "DeletedUtc",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "FxRate",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "SettledAmount",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "UnitPriceCurrency",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DeletedUtc",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PortfolioId",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "Assets",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "Assets",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "AssetHistories",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "Activities",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "TransferGroupId",
                table: "Activities",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "OccurredOn",
                table: "Activities",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "BookCurrency",
                table: "Activities",
                newName: "PriceCurrency");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "Accounts",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DisplayCurrency",
                table: "Accounts",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "Accounts",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "Assets",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume",
                table: "AssetHistories",
                type: "numeric(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Open",
                table: "AssetHistories",
                type: "numeric(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Low",
                table: "AssetHistories",
                type: "numeric(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "High",
                table: "AssetHistories",
                type: "numeric(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Close",
                table: "AssetHistories",
                type: "numeric(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AdjustedClose",
                table: "AssetHistories",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Tax",
                table: "Activities",
                type: "numeric(10,6)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FeesCurrency",
                table: "Activities",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Fees",
                table: "Activities",
                type: "numeric(10,6)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetId",
                table: "Activities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Activities",
                type: "numeric(20,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "Activities",
                type: "numeric(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Activities",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Accounts_AccountId",
                table: "Activities",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }
    }
}
