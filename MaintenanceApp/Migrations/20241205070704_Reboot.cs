using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceApp.Migrations
{
    /// <inheritdoc />
    public partial class Reboot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IncidentReportId",
                table: "ReportNotes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RoomStatusId",
                table: "IncidentReports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ReportNotes_IncidentReportId",
                table: "ReportNotes",
                column: "IncidentReportId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_RoomStatusId",
                table: "IncidentReports",
                column: "RoomStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentReports_RoomsStatus_RoomStatusId",
                table: "IncidentReports",
                column: "RoomStatusId",
                principalTable: "RoomsStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportNotes_IncidentReports_IncidentReportId",
                table: "ReportNotes",
                column: "IncidentReportId",
                principalTable: "IncidentReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncidentReports_RoomsStatus_RoomStatusId",
                table: "IncidentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportNotes_IncidentReports_IncidentReportId",
                table: "ReportNotes");

            migrationBuilder.DropIndex(
                name: "IX_ReportNotes_IncidentReportId",
                table: "ReportNotes");

            migrationBuilder.DropIndex(
                name: "IX_IncidentReports_RoomStatusId",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "IncidentReportId",
                table: "ReportNotes");

            migrationBuilder.DropColumn(
                name: "RoomStatusId",
                table: "IncidentReports");
        }
    }
}
