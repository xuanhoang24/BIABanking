using BankingSystemAPI.Application.Dtos.Admin;
using BankingSystemAPI.Domain.Entities.Users.Roles;
using BankingSystemAPI.Extensions;
using BankingSystemAPI.Application.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers.KycReviewer
{
    [ApiController]
    [Route("api/admin/kyc")]
    public class KycReviewerController : ControllerBase
    {
        private readonly IKycAdminService _kycAdminService;

        public KycReviewerController(IKycAdminService kycAdminService)
        {
            _kycAdminService = kycAdminService;
        }

        [HttpGet("pending")]
        [Authorize(Policy = PermissionCodes.KycRead)]
        public async Task<IActionResult> Pending()
        {
            var list = await _kycAdminService.GetPendingListAsync();

            return Ok(list.Select(x => new
            {
                x.Id,
                CustomerName = x.Customer!.FirstName + " " + x.Customer!.LastName,
                x.DocumentType,
                x.Status,
                x.CreatedAt,
                ReviewerName = x.ReviewedByAdminId != null
                    ? $"Admin #{x.ReviewedByAdminId}"
                    : null
            }));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = PermissionCodes.KycRead)]
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
        [Authorize(Policy = PermissionCodes.KycRead)]
        public async Task<IActionResult> GetFile(int id)
        {
            var kyc = await _kycAdminService.GetPendingAsync(id);
            if (kyc == null)
                return NotFound();

            return File(kyc.FileData, kyc.ContentType, kyc.OriginalFileName);
        }

        [HttpPost("{id}/under-review")]
        [Authorize(Policy = PermissionCodes.KycReview)]
        public async Task<IActionResult> MarkUnderReview(int id)
        {
            try
            {
                var adminId = User.GetRequiredUserId();

                await _kycAdminService.MarkUnderReviewAsync(id, adminId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/approve")]
        [Authorize(Policy = PermissionCodes.KycReview)]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var adminId = User.GetRequiredUserId();

                await _kycAdminService.ApproveAsync(id, adminId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/reject")]
        [Authorize(Policy = PermissionCodes.KycReview)]
        public async Task<IActionResult> Reject(
            int id,
            [FromBody] RejectKycRequest request)
        {
            try
            {
                var adminId = User.GetRequiredUserId();

                await _kycAdminService.RejectAsync(id, adminId, request.ReviewNotes);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
