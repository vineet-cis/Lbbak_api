using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers.Helpers;
using MediatR;
using static DataCommunication.CommonComponents.Enums;

namespace Handlers
{
    public class EditEventType
    {
        public class EditCommand : IRequest<CommonResponseTemplate>
        {
            public string Id { get; set; }
            public string? Name { get; set; }
            public int Status { get; set; }
            public int CityId { get; set; }
            public DateTime ActiveFrom { get; set; }
            public DateTime ActiveTo { get; set; }
        }

        public class Handler : IRequestHandler<EditCommand, CommonResponseTemplate>
        {
            private readonly EventDataLibrary EventDL;

            public Handler(EventDataLibrary eventDataLibrary)
            {
                EventDL = eventDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(EditCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var type = await EventDL.GetEventType(request.Id);

                    if (type != null)
                    {
                        if (!string.IsNullOrEmpty(request.Name)) type.Name = request.Name;

                        if (request.CityId != 0)
                            type.CityId = request.CityId;

                        type.Status = (Status)request.Status;
                        type.ActiveFrom = request.ActiveFrom;
                        type.ActiveTo = request.ActiveTo;

                        await EventDL.UpdateType(type);

                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Event Type Updated Successfully!",
                            data = null
                        };
                    }
                    else
                    {
                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.NotFound.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Event Type Not Found!",
                            data = null
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.InternalServerError.ToString(),
                        statusCode = HttpStatusCodes.InternalServerError,
                        msg = ex.Message.ToString(),
                        data = null
                    };
                }
            }
        }
    }
}
