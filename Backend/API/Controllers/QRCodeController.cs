using System.Xml.Linq;
using API.DTOs;
using API.Models;
using API.UnitOfWorks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

[ApiController]
[Route("api/[controller]")]
public class QrCodeController : ControllerBase
{
        public IMapper _mapper { get; }
        public IUnitOfWork _unit { get; }
        public QrCodeController(IMapper mapper, IUnitOfWork unit)
        {
            _mapper = mapper;
            _unit = unit;
        }

    [HttpPost("create")]
    //[Authorize(Roles ="Tenant")]
    public async Task<IActionResult> CreateQr([FromBody] QRDTO qrdto)
    {
        QRCode qr = _mapper.Map<QRCode>(qrdto);
        qr.ExpirationDate = DateTime.Now.AddDays(3);
        qr.GeneratedDate = DateTime.Now;

        string data = "TenantName: " + qr.TenantName
            + "\nTenantNationalId: " + qr.TenantNationalId
            + "\nOwnerName: " + qr.OwnerName
            + "\nUnitAddress: " + qr.UnitAddress
            + "\nExpirationDate: " + qr.ExpirationDate.ToString();

        if (string.IsNullOrEmpty(data))
            return BadRequest("Missing data parameter");

        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode? qrCode = new PngByteQRCode(qrData);
        byte[] qrCodeImage = qrCode.GetGraphic(20);

        qr.QRCodeValue = Convert.ToBase64String(qrCodeImage);
        await _unit.QRCodeRepository.AddAsync(qr);
        await _unit.SaveAsync();

        return File(qrCodeImage, "image/png");
        //return Ok(new { Message="image/png" });
    }

    [HttpGet("{QrId}")]
    //[Authorize(Roles = "Tenant,Owner,Admin")]
    public async Task<IActionResult> GetQRCodeById(int QrId)
    {
        QRCode? qrCode = await _unit.QRCodeRepository.GetByIdAsync(QrId);
        byte[] Qrimage= Convert.FromBase64String(qrCode.QRCodeValue);

        return File(Qrimage,"image/png");
    }
}