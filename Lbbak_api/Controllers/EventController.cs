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

        [HttpGet("GetAllEventTypes")]
        public async Task<CommonResponseTemplateWithDataArrayList<EventTypeAdminDTO>> GetAllEventTypes()
        {
            return await Mediator.Send(new GetAllEventTypes.Query());
        }

        [HttpGet("GetAllInvitations")]
        public async Task<CommonResponseTemplateWithDataArrayList<EventResponseDTO>> GetAllInvitations()
        {
            return await Mediator.Send(new GetAllInvites.Query());
        }

        [HttpPost("CreateEventType")]
        public async Task<CommonResponseTemplate> CreateEventType(CreateEventType.CreateTypeCommand type)
        {
            return await Mediator.Send(type);
        }

        [HttpPost("EditEventType")]
        public async Task<CommonResponseTemplate> EditEventType(EditEventType.EditCommand type)
        {
            return await Mediator.Send(type);
        }
    }
}
