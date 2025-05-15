using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zuricos.Folio.Api.Migrations;

/// <inheritdoc />
public partial class InitTables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Assets",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Isin = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Symbol = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                DataSource = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Assets", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Language = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                Theme = table.Column<int>(type: "integer", nullable: false),
                BaseCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AssetHistories",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                Date = table.Column<DateOnly>(type: "date", nullable: false),
                Open = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                High = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                Low = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                Close = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                Volume = table.Column<decimal>(type: "numeric(18,6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssetHistories", x => x.Id);
                table.ForeignKey(
                    name: "FK_AssetHistories_Assets_AssetId",
                    column: x => x.AssetId,
                    principalTable: "Assets",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Accounts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Accounts", x => x.Id);
                table.ForeignKey(
                    name: "FK_Accounts_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Activities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Amount = table.Column<decimal>(type: "numeric(20,8)", nullable: false),
                Price = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                PriceCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                Tax = table.Column<decimal>(type: "numeric(10,6)", nullable: false),
                Fees = table.Column<decimal>(type: "numeric(10,6)", nullable: false),
                FeesCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                ExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Activities", x => x.Id);
                table.ForeignKey(
                    name: "FK_Activities_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Activities_Assets_AssetId",
                    column: x => x.AssetId,
                    principalTable: "Assets",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Activities_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Accounts_UserId",
            table: "Accounts",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Activities_AccountId",
            table: "Activities",
            column: "AccountId");

        migrationBuilder.CreateIndex(
            name: "IX_Activities_AssetId",
            table: "Activities",
            column: "AssetId");

        migrationBuilder.CreateIndex(
            name: "IX_Activities_UserId",
            table: "Activities",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AssetHistories_AssetId",
            table: "AssetHistories",
            column: "AssetId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Activities");

        migrationBuilder.DropTable(
            name: "AssetHistories");

        migrationBuilder.DropTable(
            name: "Accounts");

        migrationBuilder.DropTable(
            name: "Assets");

        migrationBuilder.DropTable(
            name: "Users");
    }
}
