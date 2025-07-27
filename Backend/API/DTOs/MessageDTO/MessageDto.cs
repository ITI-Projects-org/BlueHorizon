namespace API.DTOs.MessageDTO
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string MessageContent { get; set; }
        public DateTime TimeStamp { get; set; }
        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }
        public bool IsRead { get; set; }

        //public string SenderPhotoUrl { get; set; }
        //public string ReceiverPhotoUrl { get; set; }



    }
}
