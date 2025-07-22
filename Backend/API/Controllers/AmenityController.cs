using System.Collections.Generic;
using API.Models;
using API.DTOs.AmenityDTOs;
using API.Repositories.Interfaces;
using API.UnitOfWorks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AmenityController : Controller
    {
        public IMapper _mapper { get; }
        public IUnitOfWork _unit { get; }
        public AmenityController(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAmenities()
        {

            IEnumerable<Amenity>? Amenitys = await _unit.AmenityRepository.GetAllAsync();
            IEnumerable<AmenityDTO>? AmenitysDTO = _mapper.Map<List<AmenityDTO>>(Amenitys);
            return Ok(AmenitysDTO);
        }
    }
}
