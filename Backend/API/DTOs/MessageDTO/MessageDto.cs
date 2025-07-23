namespace API.DTOs.MessageDTO
{
    public class MessageDto
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string MessageContent { get; set; }
        public DateTime TimeStamp { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }

        //public string SenderPhotoUrl { get; set; }
        //public string ReceiverPhotoUrl { get; set; }



    }
}
