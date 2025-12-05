using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zuricos.Folio.Migrations.Psql.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
      name: "Assets",
      columns: table => new
      {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        UserId = table.Column<Guid>(type: "uuid", nullable: false),
        Isin = table.Column<string>(
          type: "character varying(12)",
          maxLength: 12,
          nullable: false
        ),
        Name = table.Column<string>(
          type: "character varying(100)",
          maxLength: 100,
          nullable: false
        ),
        Symbol = table.Column<string>(
          type: "character varying(15)",
          maxLength: 15,
          nullable: false
        ),
        Currency = table.Column<string>(
          type: "character varying(3)",
          maxLength: 3,
          nullable: false
        ),
        DataSource = table.Column<int>(type: "integer", nullable: false),
        CreatedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        UpdatedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
        DeletedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: true
        ),
      },
      constraints: table =>
      {
        table.PrimaryKey("PK_Assets", x => x.Id);
        table.CheckConstraint(
          "CK_Assets_Currency_Format",
          "LENGTH(\"Currency\") = 3 AND \"Currency\" = UPPER(\"Currency\")"
        );
        table.CheckConstraint(
          "CK_Assets_ISIN_Format",
          "LENGTH(\"Isin\") <= 12 AND \"Isin\" ~ '^[A-Z]{2}[A-Z0-9]{9}[0-9]{1}$'"
        );
        table.CheckConstraint("CK_Assets_Name_NotEmpty", "LENGTH(TRIM(\"Name\")) > 0");
        table.CheckConstraint(
          "CK_Assets_SoftDelete_Integrity",
          "(\"IsDeleted\" = false AND \"DeletedUtc\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedUtc\" IS NOT NULL)"
        );
        table.CheckConstraint("CK_Assets_Symbol_NotEmpty", "LENGTH(TRIM(\"Symbol\")) > 0");
      }
    );

    migrationBuilder.CreateTable(
      name: "Users",
      columns: table => new
      {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        Language = table.Column<string>(
          type: "character varying(5)",
          maxLength: 5,
          nullable: false
        ),
        Theme = table.Column<int>(type: "integer", nullable: false),
        BaseCurrency = table.Column<string>(
          type: "character varying(3)",
          maxLength: 3,
          nullable: false
        ),
      },
      constraints: table =>
      {
        table.PrimaryKey("PK_Users", x => x.Id);
        table.CheckConstraint(
          "CK_Users_BaseCurrency_Format",
          "LENGTH(\"BaseCurrency\") = 3 AND \"BaseCurrency\" = UPPER(\"BaseCurrency\")"
        );
        table.CheckConstraint(
          "CK_Users_Language_Format",
          "\"Language\" ~ '^[a-z]{2}(-[A-Z]{2})?$'"
        );
        table.CheckConstraint("CK_Users_Theme_Valid", "\"Theme\" IN (0, 1, 2)");
      }
    );

    migrationBuilder.CreateTable(
      name: "AssetHistories",
      columns: table => new
      {
        Id = table.Column<string>(type: "text", nullable: false),
        AssetId = table.Column<Guid>(type: "uuid", nullable: false),
        CreatedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        UpdatedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        Date = table.Column<DateOnly>(type: "date", nullable: false),
        Open = table.Column<decimal>(type: "numeric(19,4)", nullable: false),
        High = table.Column<decimal>(type: "numeric(19,4)", nullable: false),
        Low = table.Column<decimal>(type: "numeric(19,4)", nullable: false),
        Close = table.Column<decimal>(type: "numeric(19,4)", nullable: false),
        AdjustedClose = table.Column<decimal>(type: "numeric(19,4)", nullable: false),
        Volume = table.Column<decimal>(type: "numeric(20,8)", nullable: false),
        IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
        DeletedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: true
        ),
      },
      constraints: table =>
      {
        table.PrimaryKey("PK_AssetHistories", x => x.Id);
        table.CheckConstraint("CK_AssetHistories_High_Low_Relationship", "\"High\" >= \"Low\"");
        table.CheckConstraint(
          "CK_AssetHistories_OHLC_Positive",
          "\"Open\" > 0 AND \"High\" > 0 AND \"Low\" > 0 AND \"Close\" > 0 AND \"AdjustedClose\" > 0"
        );
        table.CheckConstraint(
          "CK_AssetHistories_OHLC_Range",
          "\"Open\" BETWEEN \"Low\" AND \"High\" AND \"Close\" BETWEEN \"Low\" AND \"High\""
        );
        table.CheckConstraint(
          "CK_AssetHistories_SoftDelete_Integrity",
          "(\"IsDeleted\" = false AND \"DeletedUtc\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedUtc\" IS NOT NULL)"
        );
        table.CheckConstraint("CK_AssetHistories_Volume_NonNegative", "\"Volume\" >= 0");
        table.ForeignKey(
          name: "FK_AssetHistories_Assets_AssetId",
          column: x => x.AssetId,
          principalTable: "Assets",
          principalColumn: "Id"
        );
      }
    );

    migrationBuilder.CreateTable(
      name: "Portfolios",
      columns: table => new
      {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        UserId = table.Column<Guid>(type: "uuid", nullable: false),
        Name = table.Column<string>(
          type: "character varying(120)",
          maxLength: 120,
          nullable: false
        ),
        Institution = table.Column<string>(
          type: "character varying(120)",
          maxLength: 120,
          nullable: true
        ),
        DisplayCurrency = table.Column<string>(
          type: "character varying(3)",
          maxLength: 3,
          nullable: false
        ),
        Notes = table.Column<string>(
          type: "character varying(500)",
          maxLength: 500,
          nullable: true
        ),
        CreatedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        UpdatedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
        DeletedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: true
        ),
      },
      constraints: table =>
      {
        table.PrimaryKey("PK_Portfolios", x => x.Id);
        table.CheckConstraint(
          "CK_Portfolios_DisplayCurrency_Format",
          "LENGTH(\"DisplayCurrency\") = 3 AND \"DisplayCurrency\" = UPPER(\"DisplayCurrency\")"
        );
        table.CheckConstraint("CK_Portfolios_Name_NotEmpty", "LENGTH(TRIM(\"Name\")) > 0");
        table.CheckConstraint(
          "CK_Portfolios_SoftDelete_Integrity",
          "(\"IsDeleted\" = false AND \"DeletedUtc\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedUtc\" IS NOT NULL)"
        );
        table.ForeignKey(
          name: "FK_Portfolios_Users_UserId",
          column: x => x.UserId,
          principalTable: "Users",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade
        );
      }
    );

    migrationBuilder.CreateTable(
      name: "Accounts",
      columns: table => new
      {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        UserId = table.Column<Guid>(type: "uuid", nullable: false),
        PortfolioId = table.Column<Guid>(type: "uuid", nullable: false),
        Name = table.Column<string>(
          type: "character varying(120)",
          maxLength: 120,
          nullable: false
        ),
        Type = table.Column<int>(type: "integer", nullable: false),
        DisplayCurrency = table.Column<string>(
          type: "character varying(3)",
          maxLength: 3,
          nullable: false
        ),
        Category = table.Column<string>(
          type: "character varying(80)",
          maxLength: 80,
          nullable: true
        ),
        CreatedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        UpdatedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
        DeletedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: true
        ),
      },
      constraints: table =>
      {
        table.PrimaryKey("PK_Accounts", x => x.Id);
        table.CheckConstraint(
          "CK_Accounts_DisplayCurrency_Format",
          "LENGTH(\"DisplayCurrency\") = 3 AND \"DisplayCurrency\" = UPPER(\"DisplayCurrency\")"
        );
        table.CheckConstraint("CK_Accounts_Name_NotEmpty", "LENGTH(TRIM(\"Name\")) > 0");
        table.CheckConstraint(
          "CK_Accounts_SoftDelete_Integrity",
          "(\"IsDeleted\" = false AND \"DeletedUtc\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedUtc\" IS NOT NULL)"
        );
        table.CheckConstraint("CK_Accounts_Type_Valid", "\"Type\" IN (0, 1, 2)");
        table.ForeignKey(
          name: "FK_Accounts_Portfolios_PortfolioId",
          column: x => x.PortfolioId,
          principalTable: "Portfolios",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade
        );
        table.ForeignKey(
          name: "FK_Accounts_Users_UserId",
          column: x => x.UserId,
          principalTable: "Users",
          principalColumn: "Id"
        );
      }
    );

    migrationBuilder.CreateTable(
      name: "Activities",
      columns: table => new
      {
        Id = table.Column<Guid>(type: "uuid", nullable: false),
        UserId = table.Column<Guid>(type: "uuid", nullable: false),
        AccountId = table.Column<Guid>(type: "uuid", nullable: false),
        AssetId = table.Column<Guid>(type: "uuid", nullable: true),
        Type = table.Column<int>(type: "integer", nullable: false),
        Quantity = table.Column<decimal>(type: "numeric(20,8)", nullable: false),
        Amount = table.Column<decimal>(type: "numeric(19,4)", nullable: false),
        Currency = table.Column<string>(
          type: "character varying(3)",
          maxLength: 3,
          nullable: false
        ),
        FxRate = table.Column<decimal>(type: "numeric(18,8)", nullable: true),
        SourceCurrency = table.Column<string>(
          type: "character varying(3)",
          maxLength: 3,
          nullable: true
        ),
        Tax = table.Column<decimal>(type: "numeric(19,4)", nullable: true),
        Fees = table.Column<decimal>(type: "numeric(19,4)", nullable: true),
        Description = table.Column<string>(
          type: "character varying(500)",
          maxLength: 500,
          nullable: true
        ),
        TransferGroupId = table.Column<Guid>(type: "uuid", nullable: true),
        OccurredOn = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        CreatedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        UpdatedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: false,
          defaultValueSql: "CURRENT_TIMESTAMP"
        ),
        IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
        DeletedUtc = table.Column<DateTimeOffset>(
          type: "timestamp with time zone",
          nullable: true
        ),
      },
      constraints: table =>
      {
        table.PrimaryKey("PK_Activities", x => x.Id);
        table.CheckConstraint("CK_Activities_Amount_Positive", "\"Amount\" > 0");
        table.CheckConstraint(
          "CK_Activities_Asset_Required_For_Securities",
          "(\"Type\" IN (4, 5) AND \"AssetId\" IS NOT NULL) OR \"Type\" NOT IN (4, 5)"
        );
        table.CheckConstraint(
          "CK_Activities_Currency_Format",
          "LENGTH(\"Currency\") = 3 AND \"Currency\" = UPPER(\"Currency\")"
        );
        table.CheckConstraint(
          "CK_Activities_Fees_NonNegative",
          "\"Fees\" IS NULL OR \"Fees\" >= 0"
        );
        table.CheckConstraint(
          "CK_Activities_FxRate_Positive",
          "\"FxRate\" IS NULL OR \"FxRate\" > 0"
        );
        table.CheckConstraint(
          "CK_Activities_MultiCurrency_Consistency",
          "(\"FxRate\" IS NULL AND \"SourceCurrency\" IS NULL) OR (\"FxRate\" IS NOT NULL AND \"SourceCurrency\" IS NOT NULL)"
        );
        table.CheckConstraint("CK_Activities_Quantity_NonNegative", "\"Quantity\" >= 0");
        table.CheckConstraint(
          "CK_Activities_SoftDelete_Integrity",
          "(\"IsDeleted\" = false AND \"DeletedUtc\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedUtc\" IS NOT NULL)"
        );
        table.CheckConstraint(
          "CK_Activities_SourceCurrency_Format",
          "\"SourceCurrency\" IS NULL OR (LENGTH(\"SourceCurrency\") = 3 AND \"SourceCurrency\" = UPPER(\"SourceCurrency\"))"
        );
        table.CheckConstraint("CK_Activities_Tax_NonNegative", "\"Tax\" IS NULL OR \"Tax\" >= 0");
        table.CheckConstraint(
          "CK_Activities_Type_Valid",
          "\"Type\" IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 99)"
        );
        table.ForeignKey(
          name: "FK_Activities_Accounts_AccountId",
          column: x => x.AccountId,
          principalTable: "Accounts",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade
        );
        table.ForeignKey(
          name: "FK_Activities_Assets_AssetId",
          column: x => x.AssetId,
          principalTable: "Assets",
          principalColumn: "Id"
        );
        table.ForeignKey(
          name: "FK_Activities_Users_UserId",
          column: x => x.UserId,
          principalTable: "Users",
          principalColumn: "Id"
        );
      }
    );

    migrationBuilder.InsertData(
      table: "Users",
      columns: new[] { "Id", "BaseCurrency", "Language", "Theme" },
      values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "USD", "en-US", 0 }
    );

    migrationBuilder.CreateIndex(name: "IX_Accounts_Name", table: "Accounts", column: "Name");

    migrationBuilder.CreateIndex(
      name: "IX_Accounts_PortfolioId_Type_IsDeleted",
      table: "Accounts",
      columns: new[] { "PortfolioId", "Type", "IsDeleted" }
    );

    migrationBuilder.CreateIndex(
      name: "IX_Accounts_UserId_PortfolioId_IsDeleted",
      table: "Accounts",
      columns: new[] { "UserId", "PortfolioId", "IsDeleted" }
    );

    migrationBuilder.CreateIndex(
      name: "IX_Activities_AccountId_OccurredOn_IsDeleted",
      table: "Activities",
      columns: new[] { "AccountId", "OccurredOn", "IsDeleted" }
    );

    migrationBuilder.CreateIndex(
      name: "IX_Activities_AssetId_OccurredOn",
      table: "Activities",
      columns: new[] { "AssetId", "OccurredOn" },
      filter: "\"AssetId\" IS NOT NULL"
    );

    migrationBuilder.CreateIndex(
      name: "IX_Activities_OccurredOn",
      table: "Activities",
      column: "OccurredOn"
    );

    migrationBuilder.CreateIndex(
      name: "IX_Activities_TransferGroupId",
      table: "Activities",
      column: "TransferGroupId",
      filter: "\"TransferGroupId\" IS NOT NULL"
    );

    migrationBuilder.CreateIndex(
      name: "IX_Activities_UserId_Type_OccurredOn",
      table: "Activities",
      columns: new[] { "UserId", "Type", "OccurredOn" }
    );

    migrationBuilder.CreateIndex(
      name: "IX_AssetHistories_AssetId_Date_Desc",
      table: "AssetHistories",
      columns: new[] { "AssetId", "Date" },
      descending: new[] { false, true }
    );

    migrationBuilder.CreateIndex(
      name: "IX_AssetHistories_AssetId_Date_Unique",
      table: "AssetHistories",
      columns: new[] { "AssetId", "Date", "IsDeleted" },
      unique: true
    );

    migrationBuilder.CreateIndex(
      name: "IX_AssetHistories_Date_IsDeleted",
      table: "AssetHistories",
      columns: new[] { "Date", "IsDeleted" }
    );

    migrationBuilder.CreateIndex(
      name: "IX_Assets_Currency_IsDeleted",
      table: "Assets",
      columns: new[] { "Currency", "IsDeleted" }
    );

    migrationBuilder.CreateIndex(
      name: "IX_Assets_DataSource",
      table: "Assets",
      column: "DataSource"
    );

    migrationBuilder.CreateIndex(
      name: "IX_Assets_Symbol_Currency",
      table: "Assets",
      columns: new[] { "Symbol", "Currency" }
    );

    migrationBuilder.CreateIndex(
      name: "IX_Assets_Symbol_Unique",
      table: "Assets",
      column: "Symbol",
      unique: true
    );

    migrationBuilder.CreateIndex(
      name: "IX_Assets_UserId_IsDeleted",
      table: "Assets",
      columns: new[] { "UserId", "IsDeleted" }
    );

    migrationBuilder.CreateIndex(name: "IX_Portfolios_Name", table: "Portfolios", column: "Name");

    migrationBuilder.CreateIndex(
      name: "IX_Portfolios_UserId_Institution_IsDeleted",
      table: "Portfolios",
      columns: new[] { "UserId", "Institution", "IsDeleted" }
    );

    migrationBuilder.CreateIndex(
      name: "IX_Portfolios_UserId_IsDeleted",
      table: "Portfolios",
      columns: new[] { "UserId", "IsDeleted" }
    );

    migrationBuilder.CreateIndex(
      name: "IX_Users_BaseCurrency",
      table: "Users",
      column: "BaseCurrency"
    );
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(name: "Activities");

    migrationBuilder.DropTable(name: "AssetHistories");

    migrationBuilder.DropTable(name: "Accounts");

    migrationBuilder.DropTable(name: "Assets");

    migrationBuilder.DropTable(name: "Portfolios");

    migrationBuilder.DropTable(name: "Users");
  }
}
