using DataCommunication;
using DataCommunication.DTOs;
using Handlers.Card;
using Microsoft.AspNetCore.Mvc;
namespace Lbbak_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class CardController : BaseAPIController
    {
        #region GetMethods
        [HttpGet("GetCard")]
        public async Task<CommonResponseTemplate<CardResponseDTO>> GetCard(string CardGuid)
        {
            return await Mediator.Send(new GetCard.Query
            {
                Guid = CardGuid
            });
        }

        [HttpGet("GetAllCards")]
        public async Task<CommonResponseTemplateWithDataArrayList<CardResponseDTO>> GetAllCards()
        {
            return await Mediator.Send(new GetAllCards.Query());
        }
        #endregion

        #region PostMethods
        [HttpPost("CreateCard")]
        public async Task<CommonResponseTemplate> CreateCard(CreateCard.Command card)
        {
            return await Mediator.Send(card);
        }

        [HttpPost("UpdateCard")]
        public async Task<CommonResponseTemplate> UpdateCard(UpdateCard.Command card)
        {
            return await Mediator.Send(card);
        }
        #endregion
    }
}
