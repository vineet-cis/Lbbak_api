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


        // 1. Check if Mobile Number Already Registered
        //[HttpPost("check-mobile")]
        //public async Task<IActionResult> CheckMobile([FromBody] string mobileNumber)
        //{
        //    var exists = await UserDL.GetUserByNumber(mobileNumber);
        //    return Ok(new { exists });
        //}

        //// 2. Register Individual User
        //[HttpPost("register-individual")]
        //public async Task<IActionResult> RegisterIndividual([FromBody] IndividualRegisterDto dto)
        //{
        //    if (await UserDL.GetUserByNumber(dto.MobileNumber))
        //        return BadRequest("Mobile number already registered.");

        //    var user = await UserDL.CreateUser(new User
        //    {
        //        Id = Guid.NewGuid(),
        //        MobileNumber = dto.MobileNumber,
        //        Email = dto.Email,
        //        Name = dto.FirstName + " " + dto.LastName,
        //        UserTypeId = 1, // Individual
        //        Country = dto.Country,
        //        TwoFactorEnabled = dto.TwoFactorEnabled
        //    });

        //    var individualProfile = await UserDL.CreateIndividual(new IndividualUser
        //    {
        //        Id = Guid.NewGuid(),
        //        UserId = user.Id,
        //        RegionId = dto.RegionId,
        //        ProfileImageUrl = dto.ProfileImageUrl,
        //        IBAN = dto.IBAN,
        //        DateOfBirth = dto.DateOfBirth,
        //        Gender = dto.Gender
        //    });

        //    var token = _jwtTokenGenerator.GenerateAccessToken(user);
        //    return Ok(new
        //    {
        //        token,
        //        userId = user.Id,
        //        userTypeId = user.UserTypeId,
        //        mobileNumber = user.MobileNumber,
        //        email = user.Email,
        //        fullName = user.Name,
        //        profileImageUrl = individualProfile.ProfileImageUrl,
        //        status = user.Status
        //    });
        //}


        //// 3. Register Company User
        //[HttpPost("register-company")]
        //public async Task<IActionResult> RegisterCompany([FromBody] CompanyRegisterDto dto)
        //{
        //    if (await UserDL.GetUserByNumber(dto.MobileNumber))
        //        return BadRequest("Mobile number already registered.");

        //    var user = await UserDL.CreateUser(new User
        //    {
        //        Id = Guid.NewGuid(),
        //        MobileNumber = dto.MobileNumber,
        //        UserTypeId = 2, // Company
        //        Country = dto.Country,
        //        Name = dto.CompanyName,
        //        TwoFactorEnabled = dto.TwoFactorEnabled,
        //        Email = dto.Email
        //    });

        //    var companyProfile = await UserDL.CreateCompanyUser(new CompanyUser
        //    {
        //        Id = Guid.NewGuid(),
        //        UserId = user.Id,
        //        IBAN = dto.IBAN,
        //        LogoUrl = dto.LogoUrl,
        //    });

        //    var token = _jwtTokenGenerator.GenerateAccessToken(user);
        //    return Ok(new
        //    {
        //        token,
        //        userId = user.Id,
        //        userTypeId = user.UserTypeId,
        //        mobileNumber = user.MobileNumber,
        //        email = user.Email,
        //        fullName = user.Name, // Or null for company
        //        profileImageUrl = companyProfile.LogoUrl,
        //        status = user.Status
        //    });
        //}

        //// 4. Register Designer User
        //[HttpPost("register-designer")]
        //public async Task<IActionResult> RegisterDesigner([FromBody] DesignerRegisterDto dto)
        //{
        //    if (await UserDL.GetUserByNumber(dto.MobileNumber))
        //        return BadRequest("Mobile number already registered.");

        //    var user = await UserDL.CreateUser(new User
        //    {
        //        Id = Guid.NewGuid(),
        //        MobileNumber = dto.MobileNumber,
        //        UserTypeId = 3, // Designer
        //        Country = dto.Country,
        //        Name = dto.FullName,
        //        TwoFactorEnabled = dto.TwoFactorEnabled,
        //        Email = dto.Email
        //    });

        //    var designerProfile = await UserDL.CreateDesigner(new DesignerUser
        //    {
        //        Id = Guid.NewGuid(),
        //        UserId = user.Id,
        //        DesignSpeciality = dto.DesignSpeciality,
        //        PortfolioLink = dto.PortfolioLink,
        //        Gender = dto.Gender
        //    });

        //    var token = _jwtTokenGenerator.GenerateAccessToken(user);
        //    return Ok(new
        //    {
        //        token,
        //        userId = user.Id,
        //        userTypeId = user.UserTypeId,
        //        mobileNumber = user.MobileNumber,
        //        email = user.Email,
        //        fullName = user.Name,
        //        profileImageUrl = "", // If you plan to add image later
        //        status = user.Status
        //    });

        //}

        [HttpGet("useroftype")]
        public async Task<IActionResult> GetAllUsersForType([FromQuery] int? userTypeId, [FromQuery] bool? isDeleted)
        {
            var query = await UserDL.GetAllUsersAsync(userTypeId, isDeleted);

            var result = query.Select(u => new UserResponseDto
            {
                Id = u.Id,
                MobileNumber = u.MobileNumber,
                Email = u.Email,
                UserTypeId = u.UserTypeId,
                Country = u.Country,
                FirstName = u.CountryCode,
                TwoFactorEnabled = u.TwoFactorEnabled,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                UserType = u.UserType.Name,
                FullName = u.Name,
                ProfileImageUrl = u.IndividualProfile?.ProfileImageUrl ?? u.CompanyProfile?.LogoUrl,
                Gender = u.IndividualProfile?.Gender,
                DateOfBirth = u.IndividualProfile?.DateOfBirth,
                IBAN = u.IndividualProfile?.IBAN ?? u.CompanyProfile?.IBAN,
                LogoUrl = u.CompanyProfile?.LogoUrl,
                DesignSpeciality = u.DesignerProfile?.DesignSpeciality,
                PortfolioLink = u.DesignerProfile?.PortfolioLink
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var u = await UserDL.GetUserById(id);

            if (u == null)
                return NotFound("User not found");

            var dto = new UserResponseDto
            {
                Id = u.Id,
                MobileNumber = u.MobileNumber,
                Email = u.Email,
                UserTypeId = u.UserTypeId,
                Country = u.Country,
                CountryCode = u.CountryCode,
                TwoFactorEnabled = u.TwoFactorEnabled,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                UserType = u.UserType.Name,
                FullName = u.Name,
                ProfileImageUrl = u.IndividualProfile?.ProfileImageUrl ?? u.CompanyProfile?.LogoUrl,
                Gender = u.IndividualProfile?.Gender,
                DateOfBirth = u.IndividualProfile?.DateOfBirth,
                IBAN = u.IndividualProfile?.IBAN ?? u.CompanyProfile?.IBAN,
                LogoUrl = u.CompanyProfile?.LogoUrl,
                DesignSpeciality = u.DesignerProfile?.DesignSpeciality,
                PortfolioLink = u.DesignerProfile?.PortfolioLink
            };

            return Ok(dto);
        }


        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto dto)
        //{
        //    var user = await UserDL.GetUserById(id);
        //    if (user == null || user.IsDeleted)
        //        return NotFound("User not found");

        //    user.Email = dto.Email ?? user.Email;
        //    user.Country = dto.Country ?? user.Country;
        //    user.TwoFactorEnabled = dto.TwoFactorEnabled;

        //    await UserDL.UpdateUser(user);
        //    return Ok(new { message = "User updated", userId = user.Id });

        //}

        [HttpPatch("{id}/block")]
        public async Task<IActionResult> BlockUser(Guid id)
        {
            var user = await UserDL.GetUserById(id);
            if (user == null || user.IsDeleted)
                return NotFound("User not found");

            user.IsDeleted = true;
            user.Status = "Blocked";

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

        [HttpGet("search-by-name")]
        public async Task<CommonResponseTemplateWithDataArrayList<UserResponseDto>> SearchUserByName(string Name)
        {
            return await Mediator.Send(new SearchUserByName.Query
            {
                Name = Name
            });
        }

        [HttpGet("GetUser")]
        public async Task<CommonResponseTemplate<UserResponseDto>> GetUser(Guid Id)
        {
            return await Mediator.Send(new GetUser.Query
            {
                Id = Id
            });
        }

        //[HttpGet("counts")]
        //public async Task<IActionResult> GetUserCounts()
        //{
        //    var total = await UserDL.GetTotalUsers();
        //    var individual = await UserDL.GetTotalIndividualUsers();
        //    var company = await UserDL.GetCompanyUsers();
        //    var designer = await UserDL.GetDesignerUsers();
        //    var blocked = await UserDL.GetBlockedUsers();

        //    return Ok(new { total, individual, company, designer, blocked });
        //}
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
