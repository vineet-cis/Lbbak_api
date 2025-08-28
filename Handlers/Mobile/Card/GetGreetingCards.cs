using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;
using MongoDB.Driver;

namespace Handlers
{
    public class GetGreetingCards
    {
        public class Query : IRequest<CommonResponseTemplateWithDataArrayList<CardResponseDTO>>
        { }

        public class Handler : IRequestHandler<Query, CommonResponseTemplateWithDataArrayList<CardResponseDTO>>
        {
            public CardDataLibrary CardDL { get; }

            private readonly IMapper _mapper;
            private readonly IMongoCollection<MediaFile> _mediaCollection;

            public Handler(CardDataLibrary cardDataLibrary, IMapper mapper, IMongoClient client)
            {
                CardDL = cardDataLibrary;
                _mapper = mapper;
                var db = client.GetDatabase("MediaStorage");
                _mediaCollection = db.GetCollection<MediaFile>("media");
            }

            public async Task<CommonResponseTemplateWithDataArrayList<CardResponseDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var cards = await CardDL.GetAllGreetingCards();

                    if (cards.Count == 0)
                    {
                        return new CommonResponseTemplateWithDataArrayList<CardResponseDTO>
                        {
                            responseCode = ResponseCode.Empty.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "No Cards Found!",
                            data = null
                        };
                    }

                    var mediaIds = cards.Select(c => c.ProfileMediaId)
                        .Where(id => !string.IsNullOrEmpty(id))
                        .Distinct()
                        .ToList();

                    var mediaList = await _mediaCollection.Find(m => mediaIds.Contains(m.Id)).ToListAsync();

                    var mediaDict = mediaList
                        .Where(m => !string.IsNullOrEmpty(m.Id))
                        .ToDictionary(m => m.Id!, m => m);


                    // Prepare DTOs
                    var cardDtoList = cards.Select(c =>
                    {
                        mediaDict.TryGetValue(c.ProfileMediaId ?? string.Empty, out var media);

                        return new CardResponseDTO
                        {
                            Guid = c.Guid,
                            Name = c.Name,
                            Status = c.Status,
                            EventType = c.EventType.Name,
                            ImageUrl = media?.MediaUrl,
                            Annotations = media?.Annotations,
                        };
                    }).ToList();

                    return new CommonResponseTemplateWithDataArrayList<CardResponseDTO>
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Cards Fetched Successfully!",
                        data = cardDtoList
                    };

                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplateWithDataArrayList<CardResponseDTO>
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
