using BankingSystemAPI.Application.Dtos.Customers;
using BankingSystemAPI.Configuration;
using BankingSystemAPI.Application.Services.Interfaces.Kyc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // POST: api/kyc/upload
        [HttpPost("upload")]
        [RequestSizeLimit(5_242_880)] // 5MB limit
        public async Task<IActionResult> Upload([FromForm] UploadKycDocumentRequestDto dto)
        {
            var customerId = User.GetRequiredUserId();

            // Validate file size
            if (dto.File.Length > 5_242_880) // 5MB
            {
                return BadRequest(new { message = "File size must not exceed 5MB" });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var fileExtension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Only JPG, PNG, and PDF files are allowed" });
            }

            try
            {
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/kyc/my-document
        [HttpGet("my-document")]
        public async Task<IActionResult> MyDocument()
        {
            var customerId = User.GetRequiredUserId();

            var doc = await _kycService.GetCurrentCustomerDocumentAsync(customerId);
            if (doc == null)
                return NotFound();

            return Ok(new
            {
                doc.Id,
                doc.DocumentType,
                doc.Status,
                doc.OriginalFileName,
                doc.ReviewNotes,
                doc.CreatedAt,
                FileUrl = "/api/kyc/my-document/file"
            });
        }

        // GET: api/kyc/my-document/file
        [HttpGet("my-document/file")]
        public async Task<IActionResult> GetMyDocumentFile()
        {
            var customerId = User.GetRequiredUserId();

            var doc = await _kycService.GetCurrentCustomerDocumentAsync(customerId);
            if (doc == null)
                return NotFound();

            return File(doc.FileData, doc.ContentType, doc.OriginalFileName);
        }
    }
}
