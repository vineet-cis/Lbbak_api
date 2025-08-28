using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers;
using Lbbak_api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Lbbak_api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : BaseAPIController
    {
        private readonly AdminDataLibrary AdminDL;

        public AdminController(AdminDataLibrary adminDataLibrary)
        {
            AdminDL = adminDataLibrary;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingAdmin = await AdminDL.GetAdminByEmail(dto.Email);

            if (existingAdmin != null)
                return BadRequest("Admin with this email already exists.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newAdmin = new Admin
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await AdminDL.CreateAdmin(newAdmin);

            return Ok(new
            {
                newAdmin.Id,
                newAdmin.FullName,
                newAdmin.Email,
                Message = "Admin created successfully."
            });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromServices] JwtTokenGenerator tokenService, [FromBody] AdminLoginDto dto)
        {
            var admin = await AdminDL.GetAdminByEmail(dto.Email);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(dto.Password, admin.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var accessToken = tokenService.GenerateAccessToken(admin);
            var refreshToken = "";
            var validToken = await AdminDL.ValidToken(admin.Id);

            if (validToken == null)
            {
                refreshToken = tokenService.GenerateRefreshToken();
                await AdminDL.CreateAdminRefreshToken(new RefreshToken
                {
                    AdminId = admin.Id,
                    Token = refreshToken,
                    ExpiryDate = DateTime.UtcNow.AddDays(30)
                });
            }

            return Ok(new
            {
                admin.Id,
                admin.FullName,
                admin.Email,
                AccessToken = accessToken,
                RefreshToken = validToken != null ? validToken.Token : refreshToken,
                Message = "Login successful"
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromServices] JwtTokenGenerator tokenService, [FromBody] RefreshTokenRequestDto dto)
        {
            var admin = await AdminDL.GetAdmin(dto.AdminId);

            if (admin == null)
                return Unauthorized("Admin not found or deactivated.");

            var storedToken = await AdminDL.GetStoredToken(dto.AdminId, dto.RefreshToken);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token.");

            var newAccessToken = tokenService.GenerateAccessToken(admin);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            await AdminDL.CreateAdminRefreshToken(new RefreshToken
            {
                AdminId = admin.Id,
                Token = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            });

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Message = "Token refreshed successfully"
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
        {
            var storedToken = await AdminDL.GetStoredToken(dto.AdminId, dto.RefreshToken);

            if (storedToken == null)
                return NotFound("Refresh token not found or already revoked.");

            await AdminDL.RevokToken(storedToken);

            return Ok(new
            {
                Message = "Logout successful. Refresh token revoked."
            });
        }

        #region PostMethods

        [HttpPost("EditPermissions")]
        public async Task<CommonResponseTemplate> UpdateCard(EditPermissions.Command admin)
        {
            return await Mediator.Send(admin);
        }

        #endregion
    }
}
