using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosseo.Exercises.Ticketing.Data
{
  public class Seat
  {
    public int IdSeat { get; set; }
    public string Row { get; set; }
    public int SeatNumber { get; set; }

    public int IdPhysicalSeatLayout { get; set; }

    public virtual PhysicalSeatLayout PhysicalSeatLayout { get; set; }
    public virtual List<SeatLock> SeatLocks { get; set; }
    public virtual List<Ticket> Tickets { get; set; }
  }
}
