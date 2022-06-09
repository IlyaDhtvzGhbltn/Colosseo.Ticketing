using System;
using System.Collections.Generic;
using System.Text;

namespace Ticketing.ConsoleTestClient.Exceptions
{
    public class InvalidEventIdException : Exception
    {
        public InvalidEventIdException() : base("Invalid event ID")
        {
        }
    }
}
