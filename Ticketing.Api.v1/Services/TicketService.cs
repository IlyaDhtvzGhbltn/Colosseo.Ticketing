using Colosseo.Exercises.Ticketing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing.Dto.Infrastructure;

using Ticket = Ticketing.Dto.Ticket;
using SeatLock = Ticketing.Dto.SeatLock;

namespace Ticketing.Api.v1.Services
{
    public class TicketService : ServiceBase, ITicketService
    {
        public TicketService(IFactory<TicketingDbContext> factory) : base(factory)
        { }

        public async Task<List<Ticket>> GetTicketsByEventId(int eventId)
        {
            using (var context = _factoryDb.Create()) 
            {
                var ticketsEntry = context.Tickets.Where(x => x.IdEvent == eventId).ToList();
                var tickets = new List<Ticket>();
                ticketsEntry.ForEach(x=> 
                {
                    tickets.Add(new Ticket 
                    {
                        IdTicket = x.IdTicket,
                        CreatedAt = x.CreatedAt,
                        IdEvent = x.IdEvent,
                        IdSeat = x.IdSeat,
                        OwnerName = x.OwnerName,
                        State = x.State
                    });
                });

                return tickets;
            }
        }

        public async Task<SeatLock> SetupSeatLock(int eventId, int seatId)
        {
            using (var context = _factoryDb.Create())
            {
                bool seatLockExists = context.SeatLocks
                    .FirstOrDefault(x => x.IdEvent == eventId && x.IdSeat == seatId) != null ? true : false;

                if (seatLockExists)
                {
                    return null;
                }
                else 
                {
                    var seatLock = new SeatLock
                    {
                        CreationTime = DateTime.UtcNow,
                        ValidUntil = DateTime.UtcNow.AddMinutes(1),
                        IdEvent = eventId,
                        IdSeat = seatId,
                        LockCode = $"sale_on_event_{eventId}_seat_{seatId}"
                    };
                    context.SeatLocks.Add(new Colosseo.Exercises.Ticketing.Data.SeatLock 
                    {
                        CreationTime = seatLock.CreationTime,
                        ValidUntil = seatLock.ValidUntil,
                        LockCode = seatLock.LockCode,
                        IdEvent = seatLock.IdEvent,
                        IdSeat = seatLock.IdSeat
                    });

                    context.SaveChanges();
                    return seatLock;
                }
            }
        }

        public async Task<Ticket> CreateTicket(string ownerName, int eventId, int seatId)
        {
            using (var context = _factoryDb.Create()) 
            {
                bool ticketEntryExists = context.Tickets
                    .Where(x => x.State == TicketState.Valid)
                    .FirstOrDefault(x => x.IdEvent == eventId && x.IdSeat == seatId) != null ? true : false;

                if (ticketEntryExists)
                {
                    return null;
                }
                else 
                {
                    var ticket = new Ticket 
                    {
                        CreatedAt = DateTime.UtcNow, 
                        IdEvent = eventId, 
                        IdSeat = seatId, OwnerName = ownerName, 
                        State = TicketState.Valid
                    };

                    context.Tickets.Add(new Colosseo.Exercises.Ticketing.Data.Ticket 
                    {
                        State = ticket.State,
                        CreatedAt = ticket.CreatedAt,
                        IdEvent = ticket.IdEvent,
                        IdSeat = ticket.IdSeat, 
                        OwnerName = ticket.OwnerName
                    });

                    var seatLock = context.SeatLocks.FirstOrDefault(x=>x.IdEvent == eventId && x.IdSeat == seatId);
                    if (seatLock != null)
                    {
                        context.SeatLocks.Remove(seatLock);
                    }

                    context.SaveChanges();
                    ticket.IdTicket = context.Tickets.First(x => x.IdEvent == eventId && x.IdSeat == seatId).IdTicket;

                    return ticket;
                }
            }
        }

        public async Task<bool> CancelTicket(int ticketId)
        {
            using (var context = _factoryDb.Create()) 
            {
                var ticket = context.Tickets.FirstOrDefault(x => x.IdTicket == ticketId);
                if (ticket == null)
                {
                    return false;
                }
                else 
                {
                    ticket.State = TicketState.Cancelled;
                    context.SaveChanges();
                    return true;
                }
            }
        }



        public async Task<bool> SeatExist(int seatId)
        {
            using (var context = _factoryDb.Create())
            {
                var seat = context.Seats.FirstOrDefault(x => x.IdSeat == seatId);
                return (seat == null) ? false : true;
            }
        }
    }
}
