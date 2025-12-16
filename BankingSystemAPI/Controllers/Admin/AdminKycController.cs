using BankingSystemAPI.Services.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystemAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/kyc")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminKycController : ControllerBase
    {
        private readonly IKycAdminService _kycAdminService;

        public AdminKycController(IKycAdminService kycAdminService)
        {
            _kycAdminService = kycAdminService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> Pending()
        {
            var list = await _kycAdminService.GetPendingListAsync();

            return Ok(list.Select(x => new
            {
                x.Id,
                CustomerName = x.Customer!.FirstName + " " + x.Customer!.LastName,
                x.DocumentType,
                x.CreatedAt
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var kyc = await _kycAdminService.GetPendingAsync(id);
            if (kyc == null)
                return NotFound();

            return Ok(new
            {
                kyc.Id,
                CustomerName = kyc.Customer!.FirstName + " " + kyc.Customer!.LastName,
                kyc.CustomerId,
                kyc.DocumentType,
                kyc.Status,
                kyc.OriginalFileName,
                kyc.ReviewNotes,
                kyc.CreatedAt
            });
        }

        [HttpGet("{id}/file")]
        public async Task<IActionResult> GetFile(int id)
        {
            var kyc = await _kycAdminService.GetPendingAsync(id);
            if (kyc == null)
                return NotFound();

            return File(kyc.FileData, kyc.ContentType, kyc.OriginalFileName);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var adminId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            await _kycAdminService.ApproveAsync(id, adminId);
            return Ok();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectKycRequest request)
        {
            var adminId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            await _kycAdminService.RejectAsync(id, adminId, request.ReviewNotes);
            return Ok();
        }
    }

    public class RejectKycRequest
    {
        public string ReviewNotes { get; set; } = string.Empty;
    }
}
