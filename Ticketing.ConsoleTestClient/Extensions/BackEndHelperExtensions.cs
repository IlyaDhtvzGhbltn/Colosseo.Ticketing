using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Ticketing.Dto.Requests;

namespace Ticketing.ConsoleTestClient.Extensions
{
    public static class BackEndHelperExtensions
    {
        public static string CreateNewTicketRequest(this BackEndHelper backend, 
            CommandOption eventIdOpt,
            CommandOption seatIdOpt,
            CommandOption nameOpt) 
        {
            int eventId = int.Parse(eventIdOpt.Value());
            int seatId = int.Parse(seatIdOpt.Value());
            string name = nameOpt.Value();

            var request = new CreateTicketRequest()
            {
                EventId = eventId,
                OwnerName = name,
                SeatId = seatId
            };
            string content = JsonConvert.SerializeObject(request);

            return content;
        }
    }
}
