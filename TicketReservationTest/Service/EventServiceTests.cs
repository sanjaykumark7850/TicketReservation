using NUnit.Framework;
using TicketReservation.Data;
using TicketReservation.Models;
using TicketReservation.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using TicketReservation.Services;

namespace TicketReservation.Tests
{
    [TestFixture]
    public class EventServiceTests
    {
        private TicketReservationContext _context;
        private IEventService _eventService;

        [SetUp]
        public void Setup()
        {
            // Create a new in-memory database context for each test
            var options = new DbContextOptionsBuilder<TicketReservationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ensure unique database name for each test
                .Options;

            _context = new TicketReservationContext(options);
            _eventService = new EventService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose of the context to release resources
            _context.Dispose();
        }

        [Test]
        public void GetEvents_ShouldReturnAllEvents()
        {
            // Arrange
            var events = new List<Events>
            {
                new Events { Id = 1, Name = "Concert", TotalSeats = 100, AvailableSeats = 100, Date = DateTime.Now, Venue = "Hall A" },
                new Events { Id = 2, Name = "Play", TotalSeats = 50, AvailableSeats = 20, Date = DateTime.Now.AddDays(1), Venue = "Theatre" }
            };

            _context.Events.AddRange(events);
            _context.SaveChanges();

            // Act
            var result = _eventService.GetEvents();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        // write a function that adds error

        [Test]
        public void BookTickets_ShouldCreateBooking_WhenSeatsAvailable()
        {
            // Arrange
            var eventToBook = new Events
            {
                Id = 1,
                Name = "Concert",
                TotalSeats = 100,
                AvailableSeats = 100,
                Date = DateTime.Now,
                Venue = "Hall A" // Ensure Venue is set
            };

            var bookingRequest = new BookingRequest
            {
                EventName = "Concert",
                UserName = "JohnDoe",
                Tickets = 2
            };

            _context.Events.Add(eventToBook);
            _context.SaveChanges(); // Save event to the context

            // Act
            var result = _eventService.BookTickets(bookingRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("JohnDoe", result.UserName);
            Assert.AreEqual(2, result.Tickets);
            Assert.AreEqual(eventToBook.Id, result.EventId);
            Assert.AreEqual(eventToBook.Name, result.EventName);
            Assert.AreEqual(98, eventToBook.AvailableSeats); // Check available seats after booking
        }

        [Test]
        public void CancelBooking_ShouldIncreaseAvailableSeats_WhenBookingFound()
        {
            // Arrange
            var bookingReference = "12345";
            var eventEntity = new Events
            {
                Id = 1,
                Name = "Concert",
                TotalSeats = 100,
                AvailableSeats = 45,
                Date = DateTime.Now,
                Venue = "Hall A"
            };

            // Add unique event
            _context.Events.Add(eventEntity);
            _context.SaveChanges();

            // Create a booking for the event
            var booking = new Booking
            {
                EventId = eventEntity.Id,
                BookingReference = bookingReference,
                Tickets = 5,
                UserName = "JohnDoe",
                EventName = eventEntity.Name
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // Act
            var result = _eventService.CancelBooking(bookingReference);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(50, eventEntity.AvailableSeats); // Ensure available seats are updated
        }



        [Test]
        public void CreateEvent_ShouldAddEvent_WhenValidEvent()
        {
            // Arrange
            var newEvent = new Events
            {
                Name = "New Concert",
                TotalSeats = 100,
                AvailableSeats = 100,
                Date = DateTime.Now,
                Venue = "Hall B" // Ensure this is set
            };

            // Act
            var createdEvent = _eventService.CreateEvent(newEvent);

            // Assert
            Assert.IsNotNull(createdEvent);
            Assert.AreEqual("New Concert", createdEvent.Name);
            Assert.AreEqual(100, createdEvent.TotalSeats);
            Assert.AreEqual(100, createdEvent.AvailableSeats);
            Assert.AreEqual("Hall B", createdEvent.Venue); // Assert Venue
        }

        // Additional test cases can be added here...
    }
}
