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

                    var mediaIds = cards
                        .Select(c => c.ProfileMediaId)
                        .Where(id => !string.IsNullOrEmpty(id))
                        .Distinct()
                        .ToList();

                    var mediaDict = (await _mediaCollection
                            .Find(m => mediaIds.Contains(m.Id))
                            .ToListAsync())
                        .Where(m => !string.IsNullOrEmpty(m.Id)) // filter out nulls
                        .ToDictionary(m => m.Id!, m => m);


                    // Prepare DTOs
                    var cardDtoList = cards.AsParallel().Select(c =>
                    {
                        mediaDict.TryGetValue(c.ProfileMediaId ?? string.Empty, out var media);

                        string? thumbnailBase64 = null;
                        string? imageUrl = null;
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
                                    thumbnailBase64 = $"data:{media.ContentType};base64,{Convert.ToBase64String(imageData)}";
                                }
                                else
                                {
                                    var thumb = Helper.CreateThumbnail(imageData, 150, 150);
                                    thumbnailBase64 = $"data:{media.ContentType};base64,{Convert.ToBase64String(thumb)}";
                                }
                            }
                        }

                        return new CardResponseDTO
                        {
                            Guid = c.Guid,
                            Name = c.Name,
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
