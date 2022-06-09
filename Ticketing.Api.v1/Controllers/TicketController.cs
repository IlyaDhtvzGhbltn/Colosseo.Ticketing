using Microsoft.AspNetCore.Mvc;
using Ticketing.Api.v1.Extensions;
using System.Threading.Tasks;
using Ticketing.Dto.Infrastructure;
using Ticketing.Dto.Requests;
using Colosseo.Exercises.Ticketing.Data;

using SeatLock = Ticketing.Dto.SeatLock;
using Ticket = Ticketing.Dto.Ticket;

namespace Ticketing.Api.v1.Controllers
{
    [Route("api/v1/ticket")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IEventsService _eventsService;

        public TicketController(ITicketService ticketService, IEventsService eventsService)
        {
            _ticketService = ticketService;
            _eventsService = eventsService;
        }


        
        [HttpPost]
        [Route("setlock")]
        public async Task<SeatLock> SetupSeatLock([FromBody]CreateTicketRequest request)
        {
            bool eventExist = await _eventsService.EventExist(request.EventId);
            if (!eventExist)
            {
                this.SetStatusCode(404, "Event not found");
            }
            else 
            {
                EventState state = await _eventsService.GetEventState(request.EventId);
                if (state != EventState.Available)
                {
                    this.SetStatusCode(410, "Event not Available");
                }
                else 
                {
                    bool seatExist = await _ticketService.SeatExist(request.SeatId);
                    if (!seatExist)
                    {
                        this.SetStatusCode(404, "Seat not found");
                    }
                    else 
                    {
                        SeatLock seatLock = await _ticketService.SetupSeatLock(eventId: request.EventId, seatId: request.SeatId);
                        if (seatLock == null) 
                        {
                            this.SetStatusCode(409, "SeatLock already exist");
                        }
                        return seatLock;
                    }
                }
            }
            return null;
        }


        [HttpPost]
        [Route("new")]
        public async Task<Ticket> CreateTicket([FromBody] CreateTicketRequest request)
        {
            bool eventExist = await _eventsService.EventExist(request.EventId);
            if (!eventExist)
            {
                this.SetStatusCode(404, "Event not found");
            }
            else
            {
                EventState state = await _eventsService.GetEventState(request.EventId);
                if (state != EventState.Available)
                {
                    this.SetStatusCode(410, "Event not Available");
                }
                else
                {
                    bool seatExist = await _ticketService.SeatExist(request.SeatId);
                    if (!seatExist)
                    {
                        this.SetStatusCode(404, "Seat not found");
                    }
                    else
                    {

                        Ticket ticket = await _ticketService.CreateTicket(
                            ownerName: request.OwnerName,
                            eventId: request.EventId,
                            seatId: request.SeatId);
                        if (ticket == null)
                        {
                            this.SetStatusCode(409, $"Someone else has already purchased ticket for this seat");
                        }
                        return ticket;
                    }
                }
            }
            return null;
        }


        [HttpDelete]
        [Route("cancel/{ticketId}")]
        public async Task<bool> CancelTicket([FromRoute]int ticketId)
        {
            bool ticketCanceled = await _ticketService.CancelTicket(ticketId);
            if(!ticketCanceled)
            {
                this.SetStatusCode(404, "Ticket not found");
            }
            return ticketCanceled;
        }
    }
}
