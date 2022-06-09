using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Ticketing.Api.v1.Extensions;
using Ticketing.Dto.Infrastructure;
using System.Threading.Tasks;
using Ticketing.Dto;

namespace Ticketing.Api.v1.Controllers
{
    [Route("api/v1/event")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IEventsService _eventsService;

        public EventsController(ITicketService ticketService, IEventsService eventsService)
        {
            _ticketService = ticketService;
            _eventsService = eventsService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<List<Event>> GetAllEvents() 
        {
            return await _eventsService.GetAllEvents();
        }

        [HttpGet]
        [Route("{eventId}/seats/all")]
        public async Task<List<Seat>> GetAllSeats([FromRoute] int eventId) 
        {
            var seats = await _eventsService.GetAllSeatsByEventId(eventId);
            if (seats == null)
            {
                this.SetStatusCode(404, "Event not found");
            }
            return seats;
        }

        [HttpGet]
        [Route("{eventId}/seats/free")]
        public async Task<List<Seat>> GetFreeSeats([FromRoute] int eventId) 
        {
            var seats = await _eventsService.GetFreeSeatsByEventId(eventId);
            if (seats == null)
            {
                this.SetStatusCode(404, "Event not found");
            }
            return seats;
        }

        [HttpGet]
        [Route("{eventId}/tickets")]
        public async Task<List<Ticket>> GetTickets([FromRoute] int eventId)
        {
            return await _ticketService.GetTicketsByEventId(eventId);
        }
    }
}
