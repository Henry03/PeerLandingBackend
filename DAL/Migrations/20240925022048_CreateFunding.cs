using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class CreateFunding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trn_funding",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    lender_id = table.Column<string>(type: "text", nullable: false),
                    loan_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    funded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_funding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trn_funding_MST_USER_lender_id",
                        column: x => x.lender_id,
                        principalTable: "MST_USER",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trn_funding_mst_loans_loan_id",
                        column: x => x.loan_id,
                        principalTable: "mst_loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trn_funding_lender_id",
                table: "trn_funding",
                column: "lender_id");

            migrationBuilder.CreateIndex(
                name: "IX_trn_funding_loan_id",
                table: "trn_funding",
                column: "loan_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trn_funding");
        }
    }
}
