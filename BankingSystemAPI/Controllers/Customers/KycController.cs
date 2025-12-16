using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models.DTOs.Customer;
using BankingSystemAPI.Models.Users.Customers;
using BankingSystemAPI.Services.Kyc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankingSystemAPI.Controllers.Customers
{
    [ApiController]
    [Route("api/kyc")]
    [Authorize]
    public class KycController : ControllerBase
    {
        private readonly IKycService _kycService;

        public KycController(IKycService kycService)
        {
            _kycService = kycService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadKycDocumentRequestDto dto)
        {
            var customerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("File is required");

            var doc = await _kycService.UploadAsync(
                customerId,
                dto.DocumentType,
                dto.File
            );

            return Ok(new
            {
                doc.Id,
                doc.DocumentType,
                doc.Status
            });
        }

        [HttpGet("my-documents")]
        public async Task<IActionResult> MyDocuments()
        {
            var customerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            var docs = await _kycService.GetMyDocumentsAsync(customerId);

            return Ok(docs.Select(x => new
            {
                x.Id,
                x.DocumentType,
                x.Status,
                x.ReviewNotes,
                x.CreatedAt
            }));
        }
    }
}
