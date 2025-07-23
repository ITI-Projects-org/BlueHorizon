using API.DTOs;
using API.Models;
using API.UnitOfWorks;
using AutoMapper;
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
    [HttpPost]
    public IActionResult CreateQr([FromBody] QRDTO qrdto)
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
        using var qrCode = new PngByteQRCode(qrData);
        byte[] qrCodeImage = qrCode.GetGraphic(20);

        return File(qrCodeImage, "image/png");
    }
}