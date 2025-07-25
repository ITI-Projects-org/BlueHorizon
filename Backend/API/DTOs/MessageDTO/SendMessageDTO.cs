using API.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs.MessageDTO
{
    public class SendMessageDTO
    {

        public string ReceiverId { get; set; }

        //public int BookingId { get; set; }

        public string MessageContent { get; set; }
    }
}
