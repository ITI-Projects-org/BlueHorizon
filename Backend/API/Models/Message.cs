using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace API.Models
{
    public class Message
    {
        public int Id { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string Sender { get; set; }
        public virtual ApplicationUser SenderUser { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string Reciever { get; set; }
        public virtual ApplicationUser RecieverUser { get; set; }
        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }
        public string MessageContent { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsRead { get; set; }

    }
}
