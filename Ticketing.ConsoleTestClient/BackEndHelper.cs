using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Ticketing.Dto;
using Ticketing.ConsoleTestClient.Exceptions;
using Ticketing.ConsoleTestClient.Extensions;

namespace Ticketing.ConsoleTestClient
{
    public class BackEndHelper
    {
        private readonly HttpClient client;
        private readonly string _apiUrl;
        
        public BackEndHelper(string apiUrl)
        {
            _apiUrl = apiUrl;
            client = new HttpClient();
        }

        public void GetEvents(CommandLineApplication command) 
        {
            command.OnExecute(() => 
            {
                string url = $"{_apiUrl}event/all";
                string response = client.GetStringAsync(url).GetAwaiter().GetResult();
                var events = JsonConvert.DeserializeObject<List<Event>>(response);

                events.ForEach(e =>
                {
                    Console.WriteLine($"Event Id : {e.IdEvent}");
                    Console.WriteLine($"Event name : {e.Name}");
                    Console.WriteLine($"Event starts : {e.EventStart}");
                    Console.WriteLine($"Event state : {e.State}");
                    Console.WriteLine("**********************************");
                });
            });
        }        
        
        public void GetSeats(CommandLineApplication command) 
        {
            CommandOption eventIdOpt = command.Option("-eventid", "", CommandOptionType.SingleValue);
            CommandOption freeOpt = command.Option("-free", "", CommandOptionType.SingleOrNoValue);

            command.OnExecute(()=> 
            {
                bool freeSeatsOnly;
                int eventId;
                string eventIdStr = eventIdOpt.Value();
                string freeSeats = freeOpt.Value();

                if (string.IsNullOrWhiteSpace(eventIdStr))
                    throw new NullReferenceException("Event ID required");

                int.TryParse(eventIdStr, out eventId);
                if (eventId <= 0)
                    throw new InvalidEventIdException();

                if (string.IsNullOrWhiteSpace(freeSeats))
                {
                    freeSeatsOnly = false;
                }
                else
                {
                    bool.TryParse(freeSeats, out freeSeatsOnly);
                }
                string url = string.Empty;

                if (freeSeatsOnly)
                {
                    url = $"{_apiUrl}event/{eventId}/seats/free";
                }
                else 
                {
                    url = $"{_apiUrl}event/{eventId}/seats/all";
                }
                string response = client.GetStringAsync(url).GetAwaiter().GetResult();
                var seats = JsonConvert.DeserializeObject<List<Seat>>(response);
                seats.ForEach((s)=> 
                {
                    Console.WriteLine($"Seat ID : {s.IdSeat}");
                    Console.WriteLine($"Seat Row : {s.Row}");
                    Console.WriteLine($"Seat Number : {s.SeatNumber}");
                    Console.WriteLine("**********************************");
                });
            });
        }

        public void GetTickets(CommandLineApplication command)
        {
            CommandOption eventIdOpt = command.Option("-eventid", "", CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                string eventIdStr = eventIdOpt.Value();
                if (string.IsNullOrEmpty(eventIdStr))
                    throw new InvalidEventIdException();
                int eventId;
                int.TryParse(eventIdStr, out eventId);
                if (eventId <= 0)
                    throw new InvalidEventIdException();

                string resp = client.GetStringAsync($"{_apiUrl}event/{eventId}/tickets").GetAwaiter().GetResult();
                var tickets = JsonConvert.DeserializeObject<List<Ticket>>(resp);

                tickets.ForEach(t=> 
                {
                    Console.WriteLine($"Event ID : {t.IdEvent}");
                    Console.WriteLine($"Owner : {t.OwnerName}");
                    Console.WriteLine($"Created At : {t.CreatedAt}");
                    Console.WriteLine($"Seat ID : {t.IdSeat}");
                    Console.WriteLine($"State : {t.State}");
                    Console.WriteLine("**********************************");
                });
            });
        }

        public void LockSeat(CommandLineApplication command) 
        {
            CommandOption eventIdOpt = command.Option("-eventid", "", CommandOptionType.SingleValue);
            CommandOption seatIdOpt = command.Option("-seatid", "", CommandOptionType.SingleValue);
            CommandOption nameOpt = command.Option("-name", "", CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                var requestContent = this.CreateNewTicketRequest(eventIdOpt, seatIdOpt, nameOpt);
                string uri = $"{_apiUrl}ticket/setlock";
                var resp = client.PostAsync(uri, new StringContent(requestContent, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();

                Console.WriteLine(resp.StatusCode);
            });
        }        
        
        public void CreateTicket(CommandLineApplication command) 
        {
            CommandOption eventIdOpt = command.Option("-eventid", "", CommandOptionType.SingleValue);
            CommandOption seatIdOpt = command.Option("-seatid", "", CommandOptionType.SingleValue);
            CommandOption nameOpt = command.Option("-name", "", CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                var requestContent = this.CreateNewTicketRequest(eventIdOpt, seatIdOpt, nameOpt);
                string uri = $"{_apiUrl}ticket/new";
                var resp = client.PostAsync(uri, new StringContent(requestContent, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                Console.WriteLine(resp.StatusCode);
            });
        }        
        


        
        public void CancelTicket(CommandLineApplication command) 
        {
            CommandOption ticketIdOpt = command.Option("-ticketid", "", CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                int ticketId = int.Parse(ticketIdOpt.Value());
                string uri = $"{_apiUrl}ticket/cancel/{ticketId}";
                var resp = client.DeleteAsync(uri).GetAwaiter().GetResult();
                Console.WriteLine(resp.StatusCode);
            });
        }        
    }
}
