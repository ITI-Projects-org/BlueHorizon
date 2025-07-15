using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Village_System.Models
{
    public class Message
    {
        public int Id { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string Sender { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string Reciever { get; set; }
        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public string MessageContent { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsRead { get; set; }

    }
}
