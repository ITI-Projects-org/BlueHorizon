using AutoMapper;
using Village_System.DTOs.AuthenticationDTO;
using Village_System.DTOs.UnitDTO;
using Village_System.Models;

namespace Village_System.Mappers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            
            CreateMap<RegisterDTO,Tenant>().ReverseMap();
            CreateMap<RegisterDTO, Owner>().ReverseMap();

            #region Unit Mapping

            //unit to UnitDTO ==> UnitDetails
            CreateMap<Unit, UnitDetailsDTO>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.NormalizedUserName));

            // UnitDTO to Unit ==> UpdateUnit
            CreateMap<UnitDetailsDTO, Unit>()
                .ForMember(dest => dest.Owner, opt => opt.Ignore()) // Ignore Owner for now
                .ForMember(dest => dest.UnitAmenities, opt => opt.Ignore()); // Ignore UnitAmenities for now
            #endregion

        }
    }
}
