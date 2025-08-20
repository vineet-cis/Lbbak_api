using DataCommunication;
using Handlers.Event;
using Microsoft.AspNetCore.Mvc;

namespace Lbbak_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : BaseAPIController
    {
        [HttpPost("CreateInvitation")]
        public async Task<CommonResponseTemplate> CreateInvitation(CreateInvitation.Command invitation)
        {
            return await Mediator.Send(invitation);
        }
    }
}
