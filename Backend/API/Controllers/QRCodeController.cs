using System.Xml.Linq;
using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.UnitOfWorks;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

[ApiController]
[Route("api/[controller]")]
public class QrCodeController : ControllerBase
{
    public IMapper _mapper { get; }
    public IUnitOfWork _unit { get; }

    private IPhotoService _photoService;

    public QrCodeController(IMapper mapper, IUnitOfWork unit, IPhotoService photoService)
    {
        _mapper = mapper;
        _unit = unit;
        _photoService = photoService;
    }

    // ------------------------------------
    // ------------------------------------
    // ------------------------------------
    // -------------ON CLOUD---------------
    // ------------------------------------
    // ------------------------------------
    // ------------------------------------
    [HttpPost("createCloud")]
    public async Task<IActionResult> CreateQrCloud([FromBody] QRDTO qrdto)
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
        IFormFile file = new FormFile(new MemoryStream(qrCodeImage), 0, qrCodeImage.Length, "file", "qr.png");
        ImageUploadResult? QrImageUploadResult = await _photoService.AddPhotoAsync(file);
        if (QrImageUploadResult.Error != null)
            return BadRequest(QrImageUploadResult.Error.Message);
        qr.ImagePath = QrImageUploadResult.Url.ToString();
        await _unit.QRCodeRepository.AddAsync(qr);
        await _unit.SaveAsync();
        return CreatedAtAction(nameof(GetQRCodeByIdCloud), new { QrId = qr.Id }, new { Message = "Qr created successfully", QrId = qr.Id, imgPath = qr.ImagePath });
    }




    [HttpGet("Cloud/{QrId}")]
    //[Authorize(Roles = "Tenant,Owner,Admin")]
    public async Task<IActionResult> GetQRCodeByIdCloud(int QrId)
    {
        QRCode? qrCode = await _unit.QRCodeRepository.GetByIdAsync(QrId);
        return Ok(new { imgPath = qrCode.ImagePath });
    }

    // ------------------------------------
    // ------------------------------------
    // ------------------------------------
    // -------------ON DATABSE-------------
    // ------------------------------------
    // ------------------------------------
    // ------------------------------------
    [HttpPost("create")]
    //[Authorize(Roles ="Owner")]
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

        //return File(qrCodeImage, "image/png");

        return CreatedAtAction(nameof(GetQRCodeById), new { QrId = qr.Id }, new { Message = "Qr created successfully", QrId = qr.Id });
    }

    [HttpGet("{QrId}")]
    //[Authorize(Roles = "Tenant,Owner,Admin")]
    public async Task<IActionResult> GetQRCodeById(int QrId)
    {
        QRCode? qrCode = await _unit.QRCodeRepository.GetByIdAsync(QrId);
        byte[] Qrimage = Convert.FromBase64String(qrCode.QRCodeValue);

        return File(Qrimage, "image/png");
    }
}