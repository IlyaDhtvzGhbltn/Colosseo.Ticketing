using Colosseo.Exercises.Ticketing.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ticketing.Dto
{
    public class Ticket
    {
        public int IdTicket { get; set; }
        public string OwnerName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public TicketState State { get; set; }

        public int IdEvent { get; set; }
        public int IdSeat { get; set; }
    }
}
