using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosseo.Exercises.Ticketing.Data
{
  public class Ticket
  {
    public int IdTicket { get; set; }
    public string OwnerName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public TicketState State { get; set; }
    
    public int IdEvent { get; set; }
    public int IdSeat { get; set; }

    public virtual Event Event { get; set; }
    public virtual Seat Seat { get; set; }
  }
}
