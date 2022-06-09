using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosseo.Exercises.Ticketing.Data
{
  public class PhysicalSeatLayout
  {
    public int IdPhysicalSeatLayout { get; set; }
    public string Name { get; set; }

    public virtual List<Seat> Seats { get; set; }
    public virtual List<Event> Events { get; set; }
  }
}
