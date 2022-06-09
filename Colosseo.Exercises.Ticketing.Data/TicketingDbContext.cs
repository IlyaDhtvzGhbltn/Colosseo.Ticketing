using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosseo.Exercises.Ticketing.Data
{
  public class TicketingDbContext : DbContext
  {
    public DbSet<Event> Events { get; set; }
    public DbSet<PhysicalSeatLayout> PhysicalSeatLayouts { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<SeatLock> SeatLocks { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    
    public TicketingDbContext()
    { }

    public TicketingDbContext(DbContextOptions<TicketingDbContext> options)
      : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<PhysicalSeatLayout>()
        .ToTable(nameof(PhysicalSeatLayout));
      modelBuilder.Entity<PhysicalSeatLayout>()
        .HasKey(psl => psl.IdPhysicalSeatLayout);

      modelBuilder.Entity<Event>()
        .ToTable(nameof(Event));
      modelBuilder.Entity<Event>()
        .HasKey(e => e.IdEvent);
      modelBuilder.Entity<Event>()
        .HasOne<PhysicalSeatLayout>(e => e.PhysicalSeatLayout)
        .WithMany(psl => psl.Events)
        .HasForeignKey(e => e.IdPhysicalSeatLayout)
        .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<Seat>()
        .ToTable(nameof(Seat));
      modelBuilder.Entity<Seat>()
        .HasKey(s => s.IdSeat);
      modelBuilder.Entity<Seat>()
        .HasOne<PhysicalSeatLayout>(s => s.PhysicalSeatLayout)
        .WithMany(psl => psl.Seats)
        .HasForeignKey(s => s.IdPhysicalSeatLayout)
        .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<SeatLock>()
        .ToTable(nameof(SeatLock));
      modelBuilder.Entity<SeatLock>()
        .HasKey(sl => sl.IdSeatLock);
      modelBuilder.Entity<SeatLock>()
        .HasOne<Event>(sl => sl.Event)
        .WithMany(e => e.SeatLocks)
        .HasForeignKey(sl => sl.IdEvent)
        .OnDelete(DeleteBehavior.Restrict);
      modelBuilder.Entity<SeatLock>()
        .HasOne<Seat>(sl => sl.Seat)
        .WithMany(s => s.SeatLocks)
        .HasForeignKey(sl => sl.IdSeat)
        .OnDelete(DeleteBehavior.Restrict);
      modelBuilder.Entity<SeatLock>()
        .HasIndex(sl => new { sl.IdEvent, sl.IdSeat })
        .IsUnique();

      modelBuilder.Entity<Ticket>()
        .ToTable(nameof(Ticket));
      modelBuilder.Entity<Ticket>()
        .HasKey(t => t.IdTicket);
      modelBuilder.Entity<Ticket>()
        .HasOne<Event>(t => t.Event)
        .WithMany(e => e.Tickets)
        .HasForeignKey(t => t.IdEvent)
        .OnDelete(DeleteBehavior.Restrict);
      modelBuilder.Entity<Ticket>()
        .HasOne<Seat>(t => t.Seat)
        .WithMany(s => s.Tickets)
        .HasForeignKey(t => t.IdSeat)
        .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<FreeSeat>().HasNoKey();
      modelBuilder.HasDbFunction(typeof(TicketingDbContext).GetMethod(nameof(GetFreeSeatsForEvent), new[] { typeof(int) }))
        .HasName(nameof(GetFreeSeatsForEvent));
    }

    public IQueryable<FreeSeat> GetFreeSeatsForEvent(int idEvent) => FromExpression(() => GetFreeSeatsForEvent(idEvent));

    public DateTimeOffset GetSqlServerTime()
    {
      var connection = Database.GetDbConnection();
      var cmd = connection.CreateCommand();
      cmd.CommandText = "SELECT SYSDATETIMEOFFSET()";
      connection.Open();
      var time = (DateTimeOffset)cmd.ExecuteScalar();
      connection.Close();
      return time;
    }

    public int DeleteOldSeatLocks() => Database.ExecuteSqlRaw("DeleteOldSeatLocks;");
  }
}
