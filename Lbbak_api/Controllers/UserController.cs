using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.User;
using Lbbak_api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Lbbak_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class UserController : BaseAPIController
    {
        public UserDataLibrary UserDL { get; }

        private readonly JwtTokenGenerator _jwtTokenGenerator;
        public UserController(UserDataLibrary userDataLibrary, JwtTokenGenerator jwtTokenGenerator)
        {
            UserDL = userDataLibrary;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPatch("{id}/block")]
        public async Task<IActionResult> BlockUser(Guid id)
        {
            var user = await UserDL.GetUserById(id);

            if (user == null || user.IsDeleted)
                return NotFound("User not found");

            user.Status = "InActive";

            await UserDL.UpdateUser(user);
            return Ok(new { message = "User blocked successfully", userId = user.Id });
        }

        [HttpPatch("{id}/unblock")]
        public async Task<IActionResult> UnblockUser(Guid id)
        {
            var user = await UserDL.GetUserById(id);

            if (user == null)
                return NotFound("User not found");

            user.IsDeleted = false;
            user.Status = "Active";

            await UserDL.UpdateUser(user);
            return Ok(new { message = "User unblocked successfully", userId = user.Id });
        }

        #region GetMethods

        [HttpGet("counts")]
        public async Task<CommonResponseTemplate<UsersCountDTO>> GetUserCounts()
        {
            return await Mediator.Send(new GetUserCount.Query());
        }

        [HttpGet("GetAllUsers")]
        public async Task<CommonResponseTemplateWithDataArrayList<UserResponseDto>> GetAllUsers()
        {
            return await Mediator.Send(new GetAllUsers.Query());
        }

        [HttpGet("GetUser")]
        public async Task<CommonResponseTemplate<UserResponseDto>> GetUser(Guid Id)
        {
            return await Mediator.Send(new GetUser.Query
            {
                Id = Id
            });
        }
        #endregion

        #region Post Methods
        [HttpPost("UpdateUser")]
        public async Task<CommonResponseTemplate> UpdateUser(UpdateUser.Command user)
        {
            return await Mediator.Send(user);
        }
        #endregion
    }
}
