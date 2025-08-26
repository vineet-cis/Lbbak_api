using DataCommunication;
using DataCommunication.DTOs;
using Handlers;
using Handlers.Event;
using Microsoft.AspNetCore.Mvc;

namespace Lbbak_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : BaseAPIController
    {
        [HttpGet("GetAllInvitations")]
        public async Task<CommonResponseTemplateWithDataArrayList<EventResponseDTO>> GetAllInvitations()
        {
            return await Mediator.Send(new GetAllInvites.Query());
        }

        [HttpPost("CreateEventType")]
        public async Task<CommonResponseTemplate> CreateCard(CreateEventType.CreateTypeCommand type)
        {
            return await Mediator.Send(type);
        }
    }
}
