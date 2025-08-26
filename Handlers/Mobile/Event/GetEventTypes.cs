using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;
using MongoDB.Driver;

namespace Handlers
{
    public class GetEventTypes
    {
        public class Query : IRequest<CommonResponseTemplateWithDataArrayList<EventTypeDTO>>
        { }

        public class Handler : IRequestHandler<Query, CommonResponseTemplateWithDataArrayList<EventTypeDTO>>
        {

            private readonly EventDataLibrary EventDL;
            private readonly IMapper _mapper;

            public Handler(EventDataLibrary eventDataLibrary, IMapper mapper)
            {
                EventDL = eventDataLibrary;
                _mapper = mapper;
            }

            public async Task<CommonResponseTemplateWithDataArrayList<EventTypeDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var types = await EventDL.GetEventTypes();

                    if (types.Count == 0)
                    {
                        return new CommonResponseTemplateWithDataArrayList<EventTypeDTO>
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
                        return new EventTypeDTO
                        {
                            Id = c.Id,
                            Name = c.Name,
                        };

                    }).ToList();

                    return new CommonResponseTemplateWithDataArrayList<EventTypeDTO>
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Types Fetched Successfully!",
                        data = _mapper.Map<List<EventTypeDTO>, List<EventTypeDTO>>(typesDtoList)
                    };

                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplateWithDataArrayList<EventTypeDTO>
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
