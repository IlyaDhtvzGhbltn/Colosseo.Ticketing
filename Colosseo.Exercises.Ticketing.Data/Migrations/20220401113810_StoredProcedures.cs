using Microsoft.EntityFrameworkCore.Migrations;

namespace Colosseo.Exercises.Ticketing.Data.Migrations
{
    public partial class StoredProcedures : Migration
    {
				protected override void Up(MigrationBuilder migrationBuilder)
				{
						migrationBuilder.Sql(@"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Pokojný
-- Create date: 2022-04-01
-- Description:	Returns free seats from Colosseo Exercise Ticketing Database
-- =============================================
CREATE FUNCTION GetFreeSeatsForEvent
(
	@idEvent int
)
RETURNS 
@freeSeats TABLE 
(
	IdSeat int not null,
	Row nvarchar(max),
	SeatNumber int not null
)
AS
BEGIN
	DECLARE @idPhysicalSeatLayout int;
	SELECT @idPhysicalSeatLayout = e.IdPhysicalSeatLayout
	FROM [Event] e 
	WHERE e.IdEvent = @idEvent;

	INSERT INTO @freeSeats
		SELECT s.IdSeat, s.Row, s.SeatNumber
		FROM Seat s
		LEFT JOIN SeatLock sl ON sl.IdSeat = s.IdSeat AND sl.IdEvent = @idEvent AND sl.ValidUntil > GETDATE()
		LEFT JOIN Ticket t ON t.IdSeat = s.IdSeat AND t.IdEvent = @idEvent AND t.State = 1
		WHERE s.IdPhysicalSeatLayout = @idPhysicalSeatLayout 
		GROUP BY s.IdSeat, s.Row, s.SeatNumber
		HAVING COUNT(sl.IdSeatLock) = 0 AND COUNT(t.IdTicket) = 0;
	
	RETURN 
END
GO");

						migrationBuilder.Sql(@"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Pokojný
-- Create date: 2022-04-01
-- Description:	Deletes timed out seat locks
-- =============================================
CREATE PROCEDURE DeleteOldSeatLocks
AS
BEGIN
	DELETE FROM SeatLock WHERE ValidUntil < GETDATE()
END");
				}

			protected override void Down(MigrationBuilder migrationBuilder)
			{
					migrationBuilder.Sql(@"DROP PROCEDRE DeleteOldSeatLocks");
					migrationBuilder.Sql(@"DROP FUNCTION GetFreeSeatsForEvent");
					base.Down(migrationBuilder);
			}
	}
}
