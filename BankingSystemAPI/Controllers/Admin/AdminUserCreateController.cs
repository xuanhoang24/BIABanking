using BankingSystemAPI.Application.Dtos.Admin;
using BankingSystemAPI.Application.Services.Implementations.Admin;
using BankingSystemAPI.Domain.Entities.Users.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Policy = PermissionCodes.CustomerManage)]
    public class AdminUserCreateController : ControllerBase
    {
        private readonly AdminUserService _adminService;
        public AdminUserCreateController(AdminUserService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminUsers()
        {
            var admins = await _adminService.GetAllAdminUsersAsync();
            return Ok(admins);
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<AdminRoleDto>>> GetRoles()
        {
            var roles = await _adminService.GetRolesAsync();

            var dto = roles.Select(r => new AdminRoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdminUser([FromBody] AdminUserCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var admin = await _adminService.CreateAdminUserAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.RoleId
            );

            if (admin == null)
                return BadRequest(new { message = "Unable to create admin user. Email may already exist or role is invalid." });

            return Ok(new { admin.Id });
        }
    }
}
