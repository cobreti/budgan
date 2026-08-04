using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudganInfra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColumnsMapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CardNumberColumnIndex = table.Column<int>(type: "int", nullable: false),
                    CardNumberColumnText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateInscriptionColumnIndex = table.Column<int>(type: "int", nullable: false),
                    DateInscriptionColumnText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AmountColumnIndex = table.Column<int>(type: "int", nullable: false),
                    AmountColumnText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DescriptionColumnIndex = table.Column<int>(type: "int", nullable: false),
                    DescriptionColumnText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnsMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColumnsMappingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_ColumnsMapping_ColumnsMappingId",
                        column: x => x.ColumnsMappingId,
                        principalTable: "ColumnsMapping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountRecurringTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecurringId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodInDays = table.Column<double>(type: "float", nullable: false),
                    TransactionCount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AverageAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FirstOccurrenceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LastOccurrenceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRecurringTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountRecurringTransactions_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniqueKey = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RecurringId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateInscription = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    BalanceDateOffset = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RecordType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountTransactions_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_ColumnsMappingId",
                table: "Account",
                column: "ColumnsMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecurringTransactions_AccountId",
                table: "AccountRecurringTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRecurringTransactions_RecurringId",
                table: "AccountRecurringTransactions",
                column: "RecurringId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_AccountId_UniqueKey",
                table: "AccountTransactions",
                columns: new[] { "AccountId", "UniqueKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_FileId",
                table: "AccountTransactions",
                column: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountRecurringTransactions");

            migrationBuilder.DropTable(
                name: "AccountTransactions");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "ColumnsMapping");
        }
    }
}
