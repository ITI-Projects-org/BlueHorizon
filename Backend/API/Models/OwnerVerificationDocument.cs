
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class OwnerVerificationDocument
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Owner))] 
        public string OwnerId { get; set; }
        public virtual Owner Owner { get; set; }
        public DocumentType DocumentType { get; set; }
        public string FrontNationalIdDocumentPath { get; set; }
        public string BackNationalIdDocumentPath { get; set; }
        
        public DateTime UploadDate { get; set; }
        public string NationalId { get; set; }

    }
}

