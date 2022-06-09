using McMaster.Extensions.CommandLineUtils;
using System;
using System.Configuration;

namespace Ticketing.ConsoleTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            string apiUrl = ConfigurationManager.AppSettings["apiUrl"];
            var helper = new BackEndHelper(apiUrl);

            app.Command("getevents", (command) => helper.GetEvents(command));
            app.Command("getseats", (command) => helper.GetSeats(command));
            app.Command("getickets", (command) => helper.GetTickets(command));
            app.Command("lockseat", (command) => helper.LockSeat(command));
            app.Command("createticket", (command) => helper.CreateTicket(command));

            app.Command("cancelticket", (command) => helper.CancelTicket(command));
            app.Execute(args);
        }
    }
}
