using System;
using System.Collections.Generic;
using System.Text;

namespace Ticketing.Dto.Requests
{
    public class CreateTicketRequest
    {
        public int EventId { get; set; }
        public int SeatId { get; set; }
        public string OwnerName { get; set; }
    }
}
