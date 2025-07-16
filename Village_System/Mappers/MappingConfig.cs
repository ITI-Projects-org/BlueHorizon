using AutoMapper;
using Village_System.DTOs.AuthenticationDTO;
using Village_System.Models;

namespace Village_System.Mappers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            
            CreateMap<RegisterDTO,Tenant>().ReverseMap();
            CreateMap<RegisterDTO, Owner>().ReverseMap();
            CreateMap<OwnerVerificationDocument, OwnerVerificationDTO>().ReverseMap();
        }
    }
}
