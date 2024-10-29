using System.Collections.Generic;
using TicketReservation.Models;

namespace TicketReservation.Service
{
    public interface IEventService
    {
        IEnumerable<Events> GetEvents();
        Booking BookTickets(BookingRequest request);
        bool CancelBooking(string bookingReference);
        IEnumerable<object> GetAllBookings();
        Events CreateEvent(Events newEvent);
        bool DeleteEvent(int eventId);
    }
}
