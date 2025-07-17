using AutoMapper;
using Village_System.DTOs;
using Village_System.DTOs.AuthenticationDTO;
using Village_System.DTOs.VerificationDTO;
using Village_System.Models;

namespace Village_System.Mappers
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
        }
    }
}
