using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;
using MongoDB.Driver;

namespace Handlers
{
    public class EditCard
    {
        public class Query : IRequest<CommonResponseTemplate<CardResponseDTO>>
        {
            public string Guid { get; set; }
        }

        public class Handler : IRequestHandler<Query, CommonResponseTemplate<CardResponseDTO>>
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

            public async Task<CommonResponseTemplate<CardResponseDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var card = await CardDL.GetCardByGuid(request.Guid);

                    if (card == null)
                    {
                        return new CommonResponseTemplate<CardResponseDTO>
                        {
                            responseCode = ResponseCode.Empty.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Card Not Found!",
                            data = null
                        };
                    }

                    var mediaId = card.ProfileMediaId;

                    MediaFile media = null;

                    if (!string.IsNullOrEmpty(mediaId))
                    {
                        media = await _mediaCollection.Find(m => m.Id == mediaId).FirstOrDefaultAsync();
                    }

                    string? thumbnailBase64 = null;
                    List<TextAnnotation>? annotations = null;

                    if (media != null)
                    {
                        annotations = media.Annotations;

                        var imageData = media.FlattenedData?.Length > 0
                            ? media.FlattenedData
                            : media.Data?.Length > 0
                                ? media.Data
                                : null;

                        if (imageData != null && !string.IsNullOrEmpty(media.ContentType))
                            thumbnailBase64 = $"data:{media.ContentType};base64,{Convert.ToBase64String(imageData)}";
                    }

                    var cardDto = new CardResponseDTO
                    {
                        Guid = card.Guid,
                        Id = card.Id,
                        Annotations = annotations,
                        ThumbnailBase64 = thumbnailBase64
                    };

                    return new CommonResponseTemplate<CardResponseDTO>
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Card Fetched Successfully!",
                        data = cardDto
                    };

                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplate<CardResponseDTO>
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
