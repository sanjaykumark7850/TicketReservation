using System.ComponentModel.DataAnnotations;

namespace TicketReservation.Models
{
    public class Events
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
    }

    public class Booking
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserName { get; set; }
        public string EventName {  get; set; }
        public int Tickets { get; set; }
        public string BookingReference { get; set; }
    }

    public class BookingRequest
    {
        public string EventName { get; set; }
        public string UserName { get; set; }
        public int Tickets { get; set; }
    }
    
}
