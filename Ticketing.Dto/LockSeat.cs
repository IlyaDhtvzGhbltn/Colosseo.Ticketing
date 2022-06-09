using System;
using System.Collections.Generic;
using System.Text;

namespace Ticketing.Dto
{
    public class SeatLock
    {
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset ValidUntil { get; set; }
        public string LockCode { get; set; }

        public int IdEvent { get; set; }
        public int IdSeat { get; set; }
    }
}
