using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers;
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

        [HttpGet("GetUpcomingEvents")]
        public async Task<CommonResponseTemplateWithDataArrayList<EventResponseDTO>> GetUpcomingEvents()
        {
            return await Mediator.Send(new GetUpcomingEvents.Query());
        }


        [HttpPost("CreateInvitation")]
        public async Task<CommonResponseTemplate> CreateInvitation(CreateInvitation.CreateInviteCommand invitation)
        {
            return await Mediator.Send(invitation);
        }

        [HttpPost("CreateGreeting")]
        public async Task<CommonResponseTemplate> CreateGreeting(CreateGreeting.CreateGreetCommand greet)
        {
            return await Mediator.Send(greet);
        }

        [HttpPost("CreateDocEvent")]
        public async Task<CommonResponseTemplate> CreateDocEvent(CreateDocEvent.CreateDocEventCommand doc)
        {
            return await Mediator.Send(doc);
        }

        #endregion

        #region Promotional Offer

        [HttpGet("GetOfferTypes")]
        public async Task<CommonResponseTemplateWithDataArrayList<OfferCategoryDTO>> GetOfferTypes()
        {
            return await Mediator.Send(new GetOfferTypes.Query());
        }

        [HttpGet("GetOffer")]
        public async Task<CommonResponseTemplate<OfferDTO>> GetOffer(string guid)
        {
            return await Mediator.Send(new GetOffer.Query
            {
                Guid = guid
            });
        }

        [HttpGet("GetUserOffers")]
        public async Task<CommonResponseTemplateWithDataArrayList<OfferDTO>> GetUserOffers(string guid)
        {
            return await Mediator.Send(new GetUserOffers.Query
            {
                Guid = guid
            });
        }

        [HttpGet("GetAvailableOffers")]
        public async Task<CommonResponseTemplateWithDataArrayList<OfferDTO>> GetAvailableOffers(string guid)
        {
            return await Mediator.Send(new GetAvailableOffers.Query
            {
                Guid = guid
            });
        }

        [HttpPost("CreateOfferType")]
        public async Task<CommonResponseTemplate> CreateOffer(CreateOfferType.CreateCommand type)
        {
            return await Mediator.Send(type);
        }

        [HttpPost("CreateOffer")]
        public async Task<CommonResponseTemplate> CreateOffer(CreateOffer.CreateOfferCommand offer)
        {
            return await Mediator.Send(offer);
        }
        #endregion

        #region City

        [HttpGet("GetAllCities")]
        public async Task<CommonResponseTemplateWithDataArrayList<CityDTO>> GetAllCities()
        {
            return await Mediator.Send(new GetAllCities.Query());
        }

        #endregion
    }
}
