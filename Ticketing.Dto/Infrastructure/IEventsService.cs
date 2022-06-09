using Colosseo.Exercises.Ticketing.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Dto;

namespace Ticketing.Dto.Infrastructure
{
    public interface IEventsService
    {
        Task<List<Event>> GetAllEvents();
        Task<List<Seat>> GetAllSeatsByEventId(int eventId);
        Task<List<Seat>> GetFreeSeatsByEventId(int eventId);

        Task<bool> EventExist(int eventId);
        Task<EventState> GetEventState(int eventId);
    }
}
