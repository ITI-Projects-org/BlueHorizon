using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class AccessPermission
    {
        public int Id { get; set; }
        [ForeignKey(nameof(QRCode))]
        public int QRCodeId { get; set; }
        public virtual QRCode QRCode { get; set; }
        public AccessType AccessType { get; set; }
        public TargetLocation TargetLocation { get; set; }
    }
}