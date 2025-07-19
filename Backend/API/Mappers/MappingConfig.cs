using AutoMapper;
using API.DTOs.AuthenticationDTO;
using API.DTOs.VerificationDTO;
using API.Models;
using API.DTOs.UnitsDTOs;

namespace API.Mappers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<RegisterDTO,Tenant>().ReverseMap();
            CreateMap<RegisterDTO, Owner>().ReverseMap();
            CreateMap<OwnerVerificationDocument, OwnerWithUnitVerificationDTO>().ReverseMap();
            CreateMap<Owner, OwnerWithUnitVerificationDTO>().ReverseMap();
            CreateMap<RegisterDTO,Admin>().ReverseMap();
            CreateMap<Unit, OwnerWithUnitVerificationDTO>().ReverseMap();

            CreateMap<AddUnitDTO, Unit>()
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.VerificationStatus, opt => opt.MapFrom(src => VerificationStatus.Pending))
                .ForMember(dest => dest.AverageUnitRating, opt => opt.MapFrom(src => 0.0f))
                .ForMember(dest => dest.UnitAmenities, opt => opt.Ignore())
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => DocumentType.OwnershipContract)).ReverseMap();
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
