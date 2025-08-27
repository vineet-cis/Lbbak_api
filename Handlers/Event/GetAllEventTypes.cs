using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;

namespace Handlers
{
    public class GetAllEventTypes
    {
        public class Query : IRequest<CommonResponseTemplateWithDataArrayList<EventTypeAdminDTO>>
        { }

        public class Handler : IRequestHandler<Query, CommonResponseTemplateWithDataArrayList<EventTypeAdminDTO>>
        {

            private readonly EventDataLibrary EventDL;
            private readonly IMapper _mapper;

            public Handler(EventDataLibrary eventDataLibrary, IMapper mapper)
            {
                EventDL = eventDataLibrary;
                _mapper = mapper;
            }

            public async Task<CommonResponseTemplateWithDataArrayList<EventTypeAdminDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var types = await EventDL.GetEventTypes();

                    if (types.Count == 0)
                    {
                        return new CommonResponseTemplateWithDataArrayList<EventTypeAdminDTO>
                        {
                            responseCode = ResponseCode.Empty.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "No Types Found!",
                            data = null
                        };
                    }

                    // Prepare DTOs
                    var typesDtoList = types.Select(c =>
                    {
                        return new EventTypeAdminDTO
                        {
                            Id = c.Id,
                            Name = c.Name,
                            ActiveFrom = c.ActiveFrom,
                            ActiveTo = c.ActiveTo,
                            Status = c.Status,
                            City = c.City ?? null,
                        };

                    }).ToList();

                    return new CommonResponseTemplateWithDataArrayList<EventTypeAdminDTO>
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Types Fetched Successfully!",
                        data = _mapper.Map<List<EventTypeAdminDTO>, List<EventTypeAdminDTO>>(typesDtoList)
                    };

                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplateWithDataArrayList<EventTypeAdminDTO>
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
