using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Colosseo.Exercises.Ticketing.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhysicalSeatLayout",
                columns: table => new
                {
                    IdPhysicalSeatLayout = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalSeatLayout", x => x.IdPhysicalSeatLayout);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    IdEvent = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    IdPhysicalSeatLayout = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.IdEvent);
                    table.ForeignKey(
                        name: "FK_Event_PhysicalSeatLayout_IdPhysicalSeatLayout",
                        column: x => x.IdPhysicalSeatLayout,
                        principalTable: "PhysicalSeatLayout",
                        principalColumn: "IdPhysicalSeatLayout",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seat",
                columns: table => new
                {
                    IdSeat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Row = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeatNumber = table.Column<int>(type: "int", nullable: false),
                    IdPhysicalSeatLayout = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seat", x => x.IdSeat);
                    table.ForeignKey(
                        name: "FK_Seat_PhysicalSeatLayout_IdPhysicalSeatLayout",
                        column: x => x.IdPhysicalSeatLayout,
                        principalTable: "PhysicalSeatLayout",
                        principalColumn: "IdPhysicalSeatLayout",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeatLock",
                columns: table => new
                {
                    IdSeatLock = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ValidUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LockCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdEvent = table.Column<int>(type: "int", nullable: false),
                    IdSeat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatLock", x => x.IdSeatLock);
                    table.ForeignKey(
                        name: "FK_SeatLock_Event_IdEvent",
                        column: x => x.IdEvent,
                        principalTable: "Event",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeatLock_Seat_IdSeat",
                        column: x => x.IdSeat,
                        principalTable: "Seat",
                        principalColumn: "IdSeat",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    IdTicket = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    IdEvent = table.Column<int>(type: "int", nullable: false),
                    IdSeat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.IdTicket);
                    table.ForeignKey(
                        name: "FK_Ticket_Event_IdEvent",
                        column: x => x.IdEvent,
                        principalTable: "Event",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Seat_IdSeat",
                        column: x => x.IdSeat,
                        principalTable: "Seat",
                        principalColumn: "IdSeat",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_IdPhysicalSeatLayout",
                table: "Event",
                column: "IdPhysicalSeatLayout");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_IdPhysicalSeatLayout",
                table: "Seat",
                column: "IdPhysicalSeatLayout");

            migrationBuilder.CreateIndex(
                name: "IX_SeatLock_IdEvent_IdSeat",
                table: "SeatLock",
                columns: new[] { "IdEvent", "IdSeat" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeatLock_IdSeat",
                table: "SeatLock",
                column: "IdSeat");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdEvent",
                table: "Ticket",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdSeat",
                table: "Ticket",
                column: "IdSeat");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeatLock");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Seat");

            migrationBuilder.DropTable(
                name: "PhysicalSeatLayout");
        }
    }
}
