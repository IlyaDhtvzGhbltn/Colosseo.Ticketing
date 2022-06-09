using Colosseo.Exercises.Ticketing.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ticketing.Dto
{
    public class Event
    {
        public int IdEvent { get; set; }
        public string Name { get; set; }
        public DateTimeOffset EventStart { get; set; }
        public EventState State { get; set; }
    }
}
