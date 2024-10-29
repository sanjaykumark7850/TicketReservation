using System;
using System.Collections.Generic;
using System.Linq;
using TicketReservation.Data;
using TicketReservation.Models;
using TicketReservation.Service;

namespace TicketReservation.Services
{
    public class EventService : IEventService
    {
        private readonly TicketReservationContext _context;

        public EventService(TicketReservationContext context)
        {
            _context = context;
        }

        public IEnumerable<Events> GetEvents()
        {
            return _context.Events.ToList();
        }

        public Booking BookTickets(BookingRequest request)
        {
            var evnt = _context.Events.FirstOrDefault(e => e.Name == request.EventName);

            if (evnt == null)
            {
                throw new InvalidOperationException("Event not found.");
            }

            if (evnt.AvailableSeats < request.Tickets)
            {
                throw new InvalidOperationException("Not enough seats available.");
            }

            evnt.AvailableSeats -= request.Tickets;

            var booking = new Booking
            {
                EventId = evnt.Id,
                UserName = request.UserName,
                EventName = request.EventName,
                Tickets = request.Tickets,
                BookingReference = Guid.NewGuid().ToString()
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return booking;
        }


        public bool CancelBooking(string bookingReference)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingReference == bookingReference);
            if (booking == null) return false;

            var evnt = _context.Events.Find(booking.EventId);
            if (evnt != null) evnt.AvailableSeats += booking.Tickets;

            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            return true;
        }

        public IEnumerable<object> GetAllBookings()
        {
            // Load bookings and events in memory first
            var bookings = _context.Bookings.ToList();
            var events = _context.Events.ToDictionary(e => e.Id, e => e.Name);

            // Project booking details with event names
            var bookingDetails = bookings
                .Select(b => new
                {
                    b.Id,
                    b.UserName,
                    b.Tickets,
                    b.BookingReference,
                    EventName = events.TryGetValue(b.EventId, out var eventName) ? eventName : "Unknown Event"
                })
                .ToList();

            return bookingDetails;
        }

        public Events CreateEvent(Events newEvent)
        {
            if (string.IsNullOrEmpty(newEvent.Venue))
            {
                throw new ArgumentException("Venue is required.");
            }

            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return newEvent;
        }

        public bool DeleteEvent(int eventId)
        {
            var evnt = _context.Events.FirstOrDefault(e => e.Id == eventId);
            if (evnt == null) return false;

            if (_context.Bookings.Any(b => b.EventId == eventId))
                throw new InvalidOperationException("Cannot delete the event as there are existing bookings.");

            _context.Events.Remove(evnt);
            _context.SaveChanges();

            return true;
        }
    }
}
