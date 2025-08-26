using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using Lbbak_api;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using static DataCommunication.CommonComponents.Enums;

namespace Handlers
{
    public class CreateEventType
    {
        public class CreateTypeCommand : IRequest<CommonResponseTemplate>
        {
            public string? Name { get; set; }
            public int Status { get; set; }
            public int? CityId { get; set; }
            public DateTime? ActiveFrom { get; set; }
            public DateTime? ActiveTo { get; set; }
        }

        public class CommandValidator : AbstractValidator<CreateTypeCommand>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<CreateTypeCommand, CommonResponseTemplate>
        {
            private readonly EventDataLibrary EventDL;

            public Handler(EventDataLibrary eventDataLibrary)
            {
                EventDL = eventDataLibrary;
            }
            public async Task<CommonResponseTemplate> Handle(CreateTypeCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    int cardId = await EventDL.CreateEventType(new EventType
                    {
                        Name = request.Name,
                        Status = (Status)request.Status,
                        ActiveFrom = request.ActiveFrom ?? DateTime.MinValue,
                        ActiveTo = request.ActiveTo ?? DateTime.MinValue,
                        Guid = Helper.GetGUID()
                    });

                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "EventType Created Successfully",
                        data = null
                    };
                }
                catch (Exception ex)
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
