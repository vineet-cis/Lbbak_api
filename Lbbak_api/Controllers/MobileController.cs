using DataCommunication;
using DataCommunication.DTOs;
using Handlers.Card;
using Handlers.Event;
using Handlers.Mobile.User;
using Handlers.User;
using Microsoft.AspNetCore.Mvc;

namespace Lbbak_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class MobileController : BaseAPIController
    {
        #region User Registration Flow

        [HttpGet("GetUserByNumber")]
        public async Task<CommonResponseTemplate> GetUserFromNumber(string code, string number)
        {
            return await Mediator.Send(new GetUserByNumber.Query
            {
                Code = code,
                Number = number
            });
        }

        [HttpGet("OtpCheck")]
        public async Task<CommonResponseTemplate> OtpCheck(string otp, string code, string number)
        {
            return await Mediator.Send(new OtpCheck.Query
            {
                Otp = otp,
                Code = code,
                Number = number
            });
        }

        [HttpPost("RegisterAsIndividual")]
        public async Task<CommonResponseTemplate> RegisterAsIndividual(RegisterAsIndividual.RegisterAsIndividualCommand user)
        {
            return await Mediator.Send(user);
        }

        [HttpPost("RegisterAsCompany")]
        public async Task<CommonResponseTemplate> RegisterAsCompany(RegisterAsCompany.RegisterAsCompanyCommand user)
        {
            return await Mediator.Send(user);
        }

        [HttpPost("UpdateUserMobile")]
        public async Task<CommonResponseTemplate> UpdateUser(UpdateUserMobile.UpdateUserCommand user)
        {
            return await Mediator.Send(user);
        }

        #endregion

        #region Image Upload

        [HttpPost("ImageUpload")]
        public async Task<CommonResponseTemplate> ImageUpload(ImageUpload.Command user)
        {
            return await Mediator.Send(user);
        }

        #endregion

        #region Event Management

        

        [HttpPost("CreateInvitation")]
        public async Task<CommonResponseTemplate> CreateInvitation(CreateInvitation.CreateInviteCommand invitation)
        {
            return await Mediator.Send(invitation);
        }

        #endregion

    }
}
