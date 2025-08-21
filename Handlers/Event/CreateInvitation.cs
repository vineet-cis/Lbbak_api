using DataCommunication;
using DataCommunication.DataLibraries;
using Enums = DataCommunication.CommonComponents.Enums;
using FluentValidation;
using Handlers.Helpers;
using MediatR;

namespace Handlers.Event
{

    public class CreateInvitation
    {
        public class CreateInviteCommand : IRequest<CommonResponseTemplate>
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public Guid OwnerId { get; set; }
        }

        public class CommandValidator : AbstractValidator<CreateInviteCommand>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<CreateInviteCommand, CommonResponseTemplate>
        {
            private readonly EventDataLibrary EventDL;

            public Handler(EventDataLibrary eventDataLibrary)
            {
                EventDL = eventDataLibrary;
            }
            public async Task<CommonResponseTemplate> Handle(CreateInviteCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var invitation = new DataCommunication.Event
                    {
                        Category = Enums.EventCategory.Invitation,
                        Privacy = Enums.Privacy.Private,
                        Name = request.Name,
                        Description = request.Description,
                        EventOwnerId = request.OwnerId,
                        Guid = Helper.GetGUID()
                    };

                    await EventDL.CreateEvent(invitation);

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
