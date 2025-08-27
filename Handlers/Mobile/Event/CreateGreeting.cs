using DataCommunication;
using DataCommunication.CommonComponents;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using Lbbak_api;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Handlers
{
    public class CreateGreeting
    {
        public class CreateGreetCommand : IRequest<CommonResponseTemplate>
        {
            public IFormFile formFile { get; set; }
            public string? Name { get; set; }
            public string? Message { get; set; }
            public string? AnnotationsJson { get; set; }
            public string? RecipientName { get; set; }
            public string? RecipientNumber { get; set; }
            public DateTime ScheduleDate { get; set; }
            public Guid OwnerId { get; set; }
            public double CashGift { get; set; }
        }

        public class CommandValidator : AbstractValidator<CreateGreetCommand>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<CreateGreetCommand, CommonResponseTemplate>
        {
            private readonly EventDataLibrary EventDL;
            private readonly IMediaService _media;

            public Handler(EventDataLibrary eventDataLibrary, IMediaService mediaService)
            {
                EventDL = eventDataLibrary;
                _media = mediaService;
            }
            public async Task<CommonResponseTemplate> Handle(CreateGreetCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var annotations = !string.IsNullOrEmpty(request.AnnotationsJson)
                    ? JsonSerializer.Deserialize<List<TextAnnotation>>(request.AnnotationsJson, options)
                    : new List<TextAnnotation>();

                    string mediaId = "";

                    if (request.formFile != null && request.formFile.Length > 0)
                        mediaId = await _media.UploadAsync(request.formFile, "Event", annotations);

                    var greeting = new DataCommunication.Event
                    {
                        Category = Enums.EventCategory.Greeting,
                        Privacy = Enums.Privacy.Private,
                        Name = request.Name,
                        EventOwnerId = request.OwnerId,
                        ScheduledOn = request.ScheduleDate,
                        Guid = Helper.GetGUID(),
                        MediaId = mediaId,
                        Congratulators = new List<EventCongratulator>
                        {
                            new EventCongratulator
                            {
                                RecipientName = request.RecipientName,
                                RecipientMobile = request.RecipientNumber,
                                Message = request.Message,
                                GiftAmount = request.CashGift
                            }
                        }
                    };

                    int id = await EventDL.CreateEvent(greeting);

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
