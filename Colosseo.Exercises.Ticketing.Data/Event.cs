using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Colosseo.Exercises.Ticketing.Data
{
  public class Event
  {
    public int IdEvent { get; set; }
    public string Name { get; set; }
    public DateTimeOffset EventStart { get; set; }
    public EventState State { get; set; }

    public int IdPhysicalSeatLayout { get; set; }
    
    public virtual PhysicalSeatLayout PhysicalSeatLayout { get; set; }
    public virtual List<Ticket> Tickets { get; set; }
    public virtual List<SeatLock> SeatLocks { get; set; }
  }
}
