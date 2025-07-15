using System.ComponentModel.DataAnnotations.Schema;

namespace Village_System.Models
{
    public class AccessPermission
    {
        public int Id { get; set; }
        [ForeignKey(nameof(QRCode))]
        public int QRCodeID { get; set; }
        public virtual QRCode QRCode { get; set; }
        public AccessType AccessType { get; set; }
        public TargetLocation TargetLocation { get; set; }
    }
}