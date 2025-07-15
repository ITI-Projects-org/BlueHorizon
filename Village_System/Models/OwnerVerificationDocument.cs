
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace Village_System.Models
{
    public class OwnerVerificationDocument
    {
        [Key]
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentPath { get; set; }
        public DateTime UploadDate { get; set; }

    }
}

