using API.Models;

namespace API.DTOs.UnitDTO
{
    public class AddUnitDTO
    {
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
        public List<int> AmenityIds { get; set; }
        public IFormFile ContractDocument { get; set; }

    }
}
