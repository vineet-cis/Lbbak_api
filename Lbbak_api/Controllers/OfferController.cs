using DataCommunication;
using DataCommunication.DTOs;
using Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Lbbak_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferController : BaseAPIController
    {
        #region Get Methods

        [HttpGet("GetOffers")]
        public async Task<CommonResponseTemplateWithDataArrayList<OfferDTO>> GetOffers()
        {
            return await Mediator.Send(new GetOffers.Query());
        }

        #endregion

        #region Post Methods

        [HttpPost("CreateOfferType")]
        public async Task<CommonResponseTemplate> CreateOffer(CreateOfferType.CreateCommand type)
        {
            return await Mediator.Send(type);
        }

        [HttpPost("DeleteOfferType")]
        public async Task<CommonResponseTemplate> DeleteOfferCategory(DeleteOfferType.DeleteCommand category)
        {
            return await Mediator.Send(category);
        }

        #endregion
    }
}
