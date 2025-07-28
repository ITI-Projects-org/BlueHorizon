using API.Models;

namespace API.DTOs.VerificationDTO
{
    public class UnitVerificationDTO
    {
        public int Id { get; set; }
        public DocumentType? DocumentType { get; set; }
        public DateTime? UploadDate { get; set; }
        public string? NationalId { get; set; }
        public string? VerificationNotes { get; set; }
        public VerificationStatus? VerificationStatus { get; set; }
        public string? BankAccountDetails { get; set; }
        public int UnitId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public UnitType? UnitType { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Sleeps { get; set; }
        public int DistanceToSea { get; set; }
        public decimal BasePricePerNight { get; set; }
        public string? Address { get; set; }
        public string? VillageName { get; set; }
        public DateTime? CreationDate { get; set; }
        public float AverageUnitRating { get; set; }
        public DocumentType? Contract { get; set; }
        public string? ContractPath { get; set; }
        public int[]? UnitAmenities { get; set; }
    }
}