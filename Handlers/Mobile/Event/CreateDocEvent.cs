using DataCommunication;
using DataCommunication.CommonComponents;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using Lbbak_api;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Handlers
{
    public class CreateDocEvent
    {
        public class CreateDocEventCommand : IRequest<CommonResponseTemplate>
        {
            public IFormFile formFile { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Venue { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public Guid OwnerId { get; set; }
            public Enums.Privacy Privacy { get; set; }
        }

        public class CommandValidator : AbstractValidator<CreateDocEventCommand>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<CreateDocEventCommand, CommonResponseTemplate>
        {
            private readonly EventDataLibrary EventDL;
            private readonly IMediaService _media;

            public Handler(EventDataLibrary eventDataLibrary, IMediaService mediaService)
            {
                EventDL = eventDataLibrary;
                _media = mediaService;
            }
            public async Task<CommonResponseTemplate> Handle(CreateDocEventCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var invitation = new DataCommunication.Event
                    {
                        Category = Enums.EventCategory.Documentry,
                        Privacy = request.Privacy,
                        Name = request.Name,
                        Description = request.Description,
                        EventOwnerId = request.OwnerId,
                        Venue = request.Venue,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        Guid = Helper.GetGUID(),
                        Invitees = new List<EventInvitee>()
                    };


                    int id = await EventDL.CreateEvent(invitation);

                    var mediaId = "";

                    if (request.formFile != null && request.formFile.Length > 0)
                        mediaId = await _media.UploadAsync(request.formFile, null, null, id, null);

                    await EventDL.UpdateEventMediaId(id, mediaId);

                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Invitation Created Successfully!",
                        data = null
                    };
                }
                catch
                (Exception ex)
                {
                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.InternalServerError.ToString(),
                        statusCode = HttpStatusCodes.InternalServerError,
                        msg = ex.Message,
                        data = null
                    };
                }
            }
        }
    }
}
