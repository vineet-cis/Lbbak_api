using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;
using MongoDB.Driver;

namespace Handlers
{
    public class GetOffer
    {
        public class Query : IRequest<CommonResponseTemplate<OfferDTO>>
        {
            public string? Guid { get; set; }
        }

        public class Handler : IRequestHandler<Query, CommonResponseTemplate<OfferDTO>>
        {
            private readonly OfferDataLibrary OfferDL;
            private readonly IMapper _mapper;
            private readonly IMongoCollection<MediaFile> _mediaCollection;

            public Handler(OfferDataLibrary offerDataLibrary, IMapper mapper, IMongoClient client)
            {
                OfferDL = offerDataLibrary;
                _mapper = mapper;
                var db = client.GetDatabase("MediaStorage");
                _mediaCollection = db.GetCollection<MediaFile>("media");
            }

            public async Task<CommonResponseTemplate<OfferDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var offer = await OfferDL.GetOffer(request.Guid);

                    if (offer == null)
                    {
                        return new CommonResponseTemplate<OfferDTO>
                        {
                            responseCode = ResponseCode.NotFound.ToString(),
                            statusCode = HttpStatusCodes.NotFound,
                            msg = "Offer Not Found!",
                            data = null
                        };
                    }

                    var media = await _mediaCollection
                        .Find(m => m.Id == offer.MediaId && !string.IsNullOrEmpty(m.Id))
                        .FirstOrDefaultAsync();

                    string? image = null;

                    if (media != null)
                    {
                        var imageData = media.FlattenedData?.Length > 0
                            ? media.FlattenedData
                            : media.Data?.Length > 0
                                ? media.Data
                                : null;

                        if (imageData != null && !string.IsNullOrEmpty(media.ContentType))
                        {
                            image = $"data:{media.ContentType};base64,{Convert.ToBase64String(imageData)}";
                        }
                    }

                    var offerDto = new OfferDTO
                    {
                        Guid = offer.Guid,
                        Title = offer.Title,
                        Description = offer.Description,
                        MapLoaction = offer.LocationLink,
                        Link = offer.Link,
                        Status = offer.Status.ToString(),
                        Scope = offer.Scope.ToString(),
                        City = offer.City?.Name ?? null,
                        StartDate = offer.StartDate,
                        EndDate = offer.EndDate,
                        Image = image,
                    };

                    return new CommonResponseTemplate<OfferDTO>
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Offer Fetched Successfully!",
                        data = offerDto
                    };

                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplate<OfferDTO>
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
