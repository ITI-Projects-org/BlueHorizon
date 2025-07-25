using API.Models;

namespace API.DTOs.UnitDTO
{
    public class UnitDTO
    {
        public int? UnitId { get; set; }
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string Owner { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public UnitType UnitType { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Sleeps { get; set; }
        public int DistanceToSea { get; set; }
        public decimal BasePricePerNight { get; set; }
        public string Address { get; set; }
        public string VillageName { get; set; }
        public DateTime CreationDate { get; set; }
        public float AverageUnitRating { get; set; }
        public string imageURL { get; set; }
    }
}
