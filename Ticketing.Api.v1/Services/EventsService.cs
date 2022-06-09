using Colosseo.Exercises.Ticketing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing.Dto.Infrastructure;
using Microsoft.EntityFrameworkCore;

using Event = Ticketing.Dto.Event;
using Seat = Ticketing.Dto.Seat;

namespace Ticketing.Api.v1.Services
{
    public class EventsService : ServiceBase, IEventsService
    {
        public EventsService(IFactory<TicketingDbContext> factory) : base(factory)
        { }

        public async Task<bool> EventExist(int eventId)
        {
            using (var context = _factoryDb.Create()) 
            {
                var eventEntry = await context.Events.FirstOrDefaultAsync(x => x.IdEvent == eventId);
                return (eventEntry == null) ? false : true;
            }
        }

        public async Task<List<Event>> GetAllEvents()
        {
            using (var context = _factoryDb.Create())
            {
                var eventsList = context.Events.ToList();
                var events = new List<Event>();
                eventsList.ForEach((item)=> 
                {
                    events.Add(new Event 
                    {
                        EventStart = item.EventStart,
                        IdEvent = item.IdEvent,
                        Name = item.Name,
                        State = item.State
                    });
                });
                return events;
            }
        }

        public async Task<List<Seat>> GetAllSeatsByEventId(int eventId)
        {
            using (var context = _factoryDb.Create()) 
            {
                var eventEntry = await context.Events
                    .Where(x => x.IdEvent == eventId)
                    .Include(x => x.PhysicalSeatLayout)
                    .ThenInclude(x => x.Seats)
                    .FirstOrDefaultAsync();
                if (eventEntry == null)
                {
                    return null;
                }
                else
                {
                    var seats = new List<Seat>();
                    eventEntry.PhysicalSeatLayout.Seats
                        .ForEach(x => 
                        {
                            seats.Add(new Seat 
                            {
                                IdSeat = x.IdSeat,
                                Row = x.Row,
                                SeatNumber = x.SeatNumber
                            });
                        });
                    return seats;
                }
            }
        }

        public async Task<EventState> GetEventState(int eventId)
        {
            using (var context = _factoryDb.Create())
            {
                var eventEntry = context.Events.First(x => x.IdEvent == eventId);
                return eventEntry.State;
            }
        }

        public async Task<List<Seat>> GetFreeSeatsByEventId(int eventId)
        {
            using (var context = _factoryDb.Create()) 
            {
                var eventEntry = await context.Events
                     .Where(x => x.IdEvent == eventId)
                     .Include(x => x.Tickets)
                     .Include(x => x.PhysicalSeatLayout)
                     .ThenInclude(x => x.Seats)
                     .FirstOrDefaultAsync();
                if (eventEntry == null)
                {
                    return null;
                }
                else 
                {
                    var seats = new List<Seat>();
                    List<int> purchasedSeatsIds = eventEntry.Tickets
                        .Where(x => x.State == TicketState.Valid)
                        .Select(x => x.IdSeat)
                        .ToList();

                    eventEntry.PhysicalSeatLayout.Seats
                        .ForEach(x =>
                        {
                            if (!purchasedSeatsIds.Contains(x.IdSeat))
                            {
                                seats.Add(new Seat
                                {
                                    IdSeat = x.IdSeat,
                                    Row = x.Row,
                                    SeatNumber = x.SeatNumber
                                });
                            }
                        });
                    return seats;
                }
            }
        }
    }
}
