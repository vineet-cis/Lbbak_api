using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;
using MongoDB.Driver;

namespace Handlers.Event
{
    public class GetAllInvites
    {
        public class Query : IRequest<CommonResponseTemplateWithDataArrayList<EventResponseDTO>>
        { }

        public class Handler : IRequestHandler<Query, CommonResponseTemplateWithDataArrayList<EventResponseDTO>>
        {
            public EventDataLibrary EventDL { get; }

            private readonly IMapper _mapper;
            private readonly IMongoCollection<MediaFile> _mediaCollection;

            public Handler(EventDataLibrary eventDataLibrary, IMapper mapper, IMongoClient client)
            {
                EventDL = eventDataLibrary;
                _mapper = mapper;
                var db = client.GetDatabase("MediaStorage");
                _mediaCollection = db.GetCollection<MediaFile>("media");
            }

            public async Task<CommonResponseTemplateWithDataArrayList<EventResponseDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var invites = await EventDL.GetAllInvites();

                    if (invites.Count > 0)
                    {
                        var inviteDtoList = invites.Select(c =>
                        {
                            return new EventResponseDTO
                            {
                                Guid = c.Guid,
                                Name = c.Name,
                                Description = c.Description,
                                TypeId = c.TypeId,
                                Status = c.Status,
                                Category = c.Category.ToString(),
                                Privacy = c.Privacy.ToString(),
                                CardId = c.CardId,
                                Congratulators = c.Congratulators,
                                Invitees = c.Invitees
                            };
                        }).ToList();


                        return new CommonResponseTemplateWithDataArrayList<EventResponseDTO>
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Invitations Fetched Successfully!",
                            data = _mapper.Map<List<EventResponseDTO>, List<EventResponseDTO>>(inviteDtoList.ToList())
                        };
                    }
                    else
                    {
                        return new CommonResponseTemplateWithDataArrayList<EventResponseDTO>
                        {
                            responseCode = ResponseCode.Empty.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "No Invitations Found!",
                            data = null
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplateWithDataArrayList<EventResponseDTO>
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
