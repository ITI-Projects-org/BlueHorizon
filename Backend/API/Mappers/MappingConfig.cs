using AutoMapper;
using API.DTOs;
using API.DTOs.AuthenticationDTO;
using API.DTOs.VerificationDTO;
using API.Models;
using API.DTOs.AmenityDTOs;

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
            CreateMap<AmenityDTO, Amenity>().ReverseMap();
            CreateMap<ReviewDTO, UnitReview>().ReverseMap();
        }
    }
}
