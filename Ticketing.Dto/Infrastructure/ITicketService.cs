using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.Dto.Infrastructure
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetTicketsByEventId(int eventId);

        Task<bool> CancelTicket(int ticketId);
        Task<Ticket> CreateTicket(string ownerName, int eventId, int seatId);
        Task<SeatLock> SetupSeatLock(int eventId, int seatId);

        Task<bool> SeatExist(int seatId);
    }
}
