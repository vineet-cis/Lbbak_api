using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;
using MongoDB.Driver;

namespace Handlers.Card
{
    public class GetAllCards
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
                    var cards = await CardDL.GetAllCards();

                    if (cards.Count > 0)
                    {
                        var mediaIds = cards
                                        .Where(c => !string.IsNullOrEmpty(c.ProfileMediaId))
                                        .Select(c => c.ProfileMediaId)
                                        .Distinct()
                                        .ToList();

                        var mediaList = await _mediaCollection
                            .Find(m => mediaIds.Contains(m.Id))
                            .ToListAsync();

                        var mediaDict = mediaList.ToDictionary(m => m.Id, m => m);

                        var cardDtoList = cards.Select(c =>
                        {
                            mediaDict.TryGetValue(c.ProfileMediaId ?? string.Empty, out var media);

                            string thumbnailBase64 = null;
                            string imageUrl = null;
                            List<TextAnnotation>? annotations = null;

                            if (media != null)
                            {
                                imageUrl = $"/Image/GetImage?id={media.Id}";
                                annotations = media.Annotations;

                                var imageData = media.FlattenedData?.Length > 0
                                    ? media.FlattenedData
                                    : media.Data?.Length > 0
                                        ? media.Data
                                        : null;

                                if (imageData != null && !string.IsNullOrEmpty(media.ContentType))
                                {
                                    if (media.ContentType.Equals("image/svg+xml", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Keep SVG as-is, no resizing
                                        thumbnailBase64 = $"data:{media.ContentType};base64,{Convert.ToBase64String(imageData)}";
                                    }
                                    else
                                    {
                                        // Create thumbnail for raster images
                                        var thumb = Helper.CreateThumbnail(imageData, 150, 150);
                                        thumbnailBase64 = $"data:{media.ContentType};base64,{Convert.ToBase64String(thumb)}";
                                    }
                                }
                            }

                            return new CardResponseDTO
                            {
                                Guid = c.Guid,
                                Name = c.Name,
                                Description = c.Description,
                                EventType = c.EventType,
                                ProfileMediaId = c.ProfileMediaId,
                                Status = c.Status,
                                ImageUrl = imageUrl,
                                Annotations = annotations,
                                ThumbnailBase64 = thumbnailBase64
                            };
                        }).ToList();


                        return new CommonResponseTemplateWithDataArrayList<CardResponseDTO>
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Cards Fetched Successfully!",
                            data = _mapper.Map<List<CardResponseDTO>, List<CardResponseDTO>>(cardDtoList.ToList())
                        };
                    }
                    else
                    {
                        return new CommonResponseTemplateWithDataArrayList<CardResponseDTO>
                        {
                            responseCode = ResponseCode.Empty.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "No Cards Found!",
                            data = null
                        };
                    }
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

            public async Task<List<TextAnnotation>> GetAnnotations(string id)
            {
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrEmpty(id))
                    return null;

                var media = await _mediaCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

                if (media == null)
                    return null;
                else
                    return media.Annotations;
            }
        }
    }
}
