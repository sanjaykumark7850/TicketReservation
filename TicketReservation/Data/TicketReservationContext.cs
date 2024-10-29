using Microsoft.EntityFrameworkCore;
using TicketReservation.Models;

namespace TicketReservation.Data
{
    public class TicketReservationContext : DbContext
    {
        public TicketReservationContext(DbContextOptions<TicketReservationContext> options) : base(options) { }

        public DbSet<Events> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Events>()
            .HasKey(e => e.Id);
        }
    }
}
