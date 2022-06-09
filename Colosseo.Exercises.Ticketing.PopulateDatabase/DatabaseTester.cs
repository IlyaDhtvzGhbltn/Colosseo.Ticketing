using Colosseo.Exercises.Ticketing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Colosseo.Exercises.Ticketing.PopulateDatabase
{
  public class DatabaseTester : IHostedService
  {
    private readonly TicketingDbContext ticketingDbContext;

    public DatabaseTester(
      TicketingDbContext ticketingDbContext)
    {
      this.ticketingDbContext = ticketingDbContext;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      Run();
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }

    private void Run()
    {
      try
      {
        InitializeDb();
        RunTests(ticketingDbContext);
      }
      catch (Exception ex)
      {
        Console.WriteLine("ERROR: Exception caught: " + ex.ToString());
      }
    }

    private void InitializeDb()
    {
      var db = ticketingDbContext;

      db.Database.Migrate();

      if (!db.PhysicalSeatLayouts.Any())
      {
        Console.WriteLine("No physical seat layouts found, populating database with test data");

        PhysicalSeatLayout defaultLayout = new PhysicalSeatLayout() { Name = "Default seat layout", Seats = new List<Seat>() };
        PhysicalSeatLayout smallLayout = new PhysicalSeatLayout() { Name = "Small seat layout", Seats = new List<Seat>() };
        PhysicalSeatLayout extendedLayout = new PhysicalSeatLayout() { Name = "Extended seat layout", Seats = new List<Seat>() };

        db.PhysicalSeatLayouts.Add(defaultLayout);
        db.PhysicalSeatLayouts.Add(smallLayout);
        db.PhysicalSeatLayouts.Add(extendedLayout);
        db.SaveChanges();

        foreach (var segmentName in new[] { "A", "B", "C" })
          for (int rowNumber = 1; rowNumber <= 10; rowNumber++)
            for (int seatNumber = 1; seatNumber <= 20; seatNumber++)
              defaultLayout.Seats.Add(new Seat() { Row = $"{segmentName}{rowNumber:00}", SeatNumber = seatNumber });
        db.SaveChanges();

        foreach (var segmentName in new[] { "A" })
          for (int rowNumber = 1; rowNumber <= 5; rowNumber++)
            for (int seatNumber = 1; seatNumber <= 10; seatNumber++)
              smallLayout.Seats.Add(new Seat() { Row = $"{segmentName}{rowNumber:00}", SeatNumber = seatNumber });
        db.SaveChanges();

        foreach (var segmentName in new[] { "A", "B", "C", "D", "E" })
          for (int rowNumber = 1; rowNumber <= 25 || (segmentName == "C" && rowNumber <= 40); rowNumber++)
            for (int seatNumber = 1; seatNumber <= 30 || (segmentName == "C" && seatNumber <= 50); seatNumber++)
              extendedLayout.Seats.Add(new Seat() { Row = $"{segmentName}{rowNumber:00}", SeatNumber = seatNumber });
        db.SaveChanges();

        db.Events.Add(new Event() { Name = "Small event #1", State = EventState.Available, EventStart = DateTime.Now.Date.AddDays(1).AddHours(15), PhysicalSeatLayout = smallLayout });
        db.Events.Add(new Event() { Name = "Small event #2", State = EventState.Available, EventStart = DateTime.Now.Date.AddDays(2).AddHours(15), PhysicalSeatLayout = smallLayout });
        db.Events.Add(new Event() { Name = "Small event #3", State = EventState.Available, EventStart = DateTime.Now.Date.AddDays(3).AddHours(15), PhysicalSeatLayout = smallLayout });
        db.Events.Add(new Event() { Name = "Normal event #1", State = EventState.Available, EventStart = DateTime.Now.Date.AddDays(1).AddHours(20), PhysicalSeatLayout = defaultLayout });
        db.Events.Add(new Event() { Name = "Normal event #2", State = EventState.Available, EventStart = DateTime.Now.Date.AddDays(1).AddHours(22), PhysicalSeatLayout = defaultLayout });
        db.Events.Add(new Event() { Name = "Large event #1", State = EventState.Available, EventStart = DateTime.Now.Date.AddDays(4).AddHours(19), PhysicalSeatLayout = extendedLayout });
        db.Events.Add(new Event() { Name = "Large event #2", State = EventState.Available, EventStart = DateTime.Now.Date.AddDays(5).AddHours(19), PhysicalSeatLayout = extendedLayout });
        db.SaveChanges();

        Console.WriteLine("Data generated.");
      }

      var pslCount = db.PhysicalSeatLayouts.Count();
      var seatCount = db.Seats.Count();
      var eventCount = db.Events.Count();

      Console.WriteLine($"Database contains {pslCount} physical seat layouts with {seatCount} seats and {eventCount} events.");
    }

    private static void RunTests(TicketingDbContext db)
    {
      Console.WriteLine("Running tests");

      var nextEvent = db.Events.Include(e => e.PhysicalSeatLayout).OrderBy(e => e.EventStart).FirstOrDefault();
      if (nextEvent == null)
        throw new ApplicationException("No event found.");

      Console.WriteLine($"Found event #{nextEvent.IdEvent} {nextEvent.Name} with seat layout {nextEvent.PhysicalSeatLayout.Name}");

      Console.Write("Deleting old seat locks... ");
      db.DeleteOldSeatLocks();
      Console.WriteLine("done");

      var freeSeats = db.GetFreeSeatsForEvent(nextEvent.IdEvent).ToList();
      Console.WriteLine($"Number of free seats: {freeSeats.Count}");

      if (!freeSeats.Any())
        throw new ApplicationException("No free seats in event.");

      var currentServerTime = db.GetSqlServerTime();

      var testSeat1 = freeSeats.First();
      Console.WriteLine("Trying to create a seat lock.");
      var sl = new SeatLock()
      {
        CreationTime = currentServerTime,
        IdEvent = nextEvent.IdEvent,
        IdSeat = testSeat1.IdSeat,
        LockCode = "sale1",
        ValidUntil = currentServerTime.AddSeconds(1)
      };
      db.SeatLocks.Add(sl);
      db.SaveChanges();

      var freeSeats2 = db.GetFreeSeatsForEvent(nextEvent.IdEvent).ToList();
      if (freeSeats2.Any(fs => fs.IdSeat == sl.IdSeat))
        throw new ApplicationException($"Error: seat #{sl.IdSeat} should be locked but it is listed as free seat.");

      Console.WriteLine("Waiting for seat lock to expire.");
      Thread.Sleep(TimeSpan.FromSeconds(2));

      var freeSeats3 = db.GetFreeSeatsForEvent(nextEvent.IdEvent).ToList();
      if (!freeSeats3.Any(fs => fs.IdSeat == sl.IdSeat))
        throw new ApplicationException($"Error: seat #{sl.IdSeat} should be free because its lock should have expired.");

      Console.WriteLine($"Trying to create a ticket for event #{nextEvent.IdEvent} and seat #{testSeat1.IdSeat}.");
      var idTicket = TicketInsertExample(db, nextEvent.IdEvent, testSeat1.IdSeat, "salecode2", "Peter");

      var freeSeats4 = db.GetFreeSeatsForEvent(nextEvent.IdEvent).ToList();
      if (freeSeats4.Any(fs => fs.IdSeat == testSeat1.IdSeat))
        throw new ApplicationException($"Error: seat #{testSeat1.IdSeat} should be unavailable because it has valid ticket, but was listed as free seat.");

      Console.WriteLine("Cancelling the ticket");
      var ticket = db.Tickets.FirstOrDefault(t => t.IdTicket == idTicket);
      if (ticket == null)
        throw new ApplicationException($"Error: unable to find ticket #{idTicket}");

      ticket.State = TicketState.Cancelled;
      db.SaveChanges();

      var freeSeats5 = db.GetFreeSeatsForEvent(nextEvent.IdEvent).ToList();
      if (!freeSeats5.Any(fs => fs.IdSeat == testSeat1.IdSeat))
        throw new ApplicationException($"Error: seat #{testSeat1.IdSeat} for event #{nextEvent.IdEvent} should be free, because its ticket was cancelled, but it was not listed as a free seat.");

      Console.WriteLine("All tests ran successfully.");
    }

    private static int TicketInsertExample(TicketingDbContext db, int idEvent, int idSeat, string lockCode, string ownerName)
    {
      // 1. Delete old seat locks
      db.DeleteOldSeatLocks();

      // 2. Try and create seat lock
      var currentServerTime = db.GetSqlServerTime();
      var seatLock = new SeatLock()
      {
        CreationTime = currentServerTime,
        IdEvent = idEvent,
        IdSeat = idSeat,
        LockCode = lockCode,
        ValidUntil = currentServerTime.AddSeconds(30)
      };
      db.SeatLocks.Add(seatLock);
      try
      {
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        throw new ApplicationException("Could not create seat lock, seat could be already locked");
      }

      try
      {
        // 3. Now that the seat is locked, check whether ticket for this event and seat already exists
        if (db.Tickets.Any(t => t.IdEvent == idEvent && t.IdSeat == idSeat && t.State == TicketState.Valid))
          throw new ApplicationException($"A valid ticket already exists for event #{idEvent} and seat #{idSeat}.");

        var ticket = new Ticket()
        {
          IdEvent = idEvent,
          IdSeat = idSeat,
          OwnerName = ownerName,
          CreatedAt = currentServerTime,
          State = TicketState.Valid
        };
        db.Tickets.Add(ticket);
        db.SaveChanges();

        // Ticket was successfully created, return its ID
        return ticket.IdTicket;
      }
      finally
      {
        // Created seat lock should be cleared whether the ticket was created or not
        db.SeatLocks.Remove(seatLock);
        db.SaveChanges();
      }
    }
  }
}
