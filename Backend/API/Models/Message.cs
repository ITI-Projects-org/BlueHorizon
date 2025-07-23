using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Message
    {
        public int Id { get; set; }

        // Sender
        public string SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public virtual ApplicationUser SenderUser { get; set; }

        // Receiver
        public string ReceiverId { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public virtual ApplicationUser ReceiverUser { get; set; }

        // Booking
        //public int BookingId { get; set; }

        //[ForeignKey(nameof(BookingId))]
        //public virtual Booking Booking { get; set; }

        public string MessageContent { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsRead { get; set; }
    }
}
