using Microsoft.AspNetCore.Mvc;
using TicketReservation.Models;
using TicketReservation.Service;

public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Events>> GetEvents()
    {
        var events = _eventService.GetEvents();
        return Ok(events);
    }

    [HttpPost("book")]
    public ActionResult<Booking> BookTickets([FromBody] BookingRequest request)
    {
        try
        {
            var booking = _eventService.BookTickets(request);
            return Ok(booking);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("cancel-booking/{bookingReference}")]
    public ActionResult CancelBooking(string bookingReference)
    {
        if (!_eventService.CancelBooking(bookingReference))
            return NotFound("Booking not found.");

        return Ok(new { message = "Booking cancelled successfully" });
    }

    [HttpGet("booking")]
    public ActionResult<IEnumerable<object>> GetAllBookings()
    {
        var bookings = _eventService.GetAllBookings();
        return Ok(bookings);
    }

    [HttpPost("create")]
    public ActionResult<Events> CreateEvent([FromBody] Events newEvent)
    {
        try
        {
            var createdEvent = _eventService.CreateEvent(newEvent);
            return Ok(createdEvent);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteEvent(int id)
    {
        try
        {
            if (!_eventService.DeleteEvent(id))
                return NotFound("Event not found.");

            return Ok("Event removed successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
