using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosseo.Exercises.Ticketing.Data
{
  public class SeatLock
  {
    public int IdSeatLock { get; set; }
    public DateTimeOffset CreationTime { get; set; }
    public DateTimeOffset ValidUntil { get; set; }
    public string LockCode { get; set; }

    public int IdEvent { get; set; }
    public int IdSeat { get; set; }

    public Event Event { get; set; }
    public Seat Seat { get; set; }
  }
}
