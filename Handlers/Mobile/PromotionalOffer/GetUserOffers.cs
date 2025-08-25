using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;
using MongoDB.Driver;

namespace Handlers
{
    public class GetUserOffers
    {
        public class Query : IRequest<CommonResponseTemplateWithDataArrayList<OfferDTO>>
        {
            public string? Guid { get; set; }
        }

        public class Handler : IRequestHandler<Query, CommonResponseTemplateWithDataArrayList<OfferDTO>>
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

            public async Task<CommonResponseTemplateWithDataArrayList<OfferDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var offers = await OfferDL.GetAllOffersForUser(request.Guid);

                    if (offers.Count == 0)
                    {
                        return new CommonResponseTemplateWithDataArrayList<OfferDTO>
                        {
                            responseCode = ResponseCode.Empty.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "No Offers Found!",
                            data = null
                        };
                    }

                    var mediaIds = offers
                        .Select(c => c.MediaId)
                        .Where(id => !string.IsNullOrEmpty(id))
                        .Distinct()
                        .ToList();

                    var mediaDict = (await _mediaCollection
                            .Find(m => mediaIds.Contains(m.Id))
                            .ToListAsync())
                        .Where(m => !string.IsNullOrEmpty(m.Id))
                        .ToDictionary(m => m.Id!, m => m);

                    var offerDtoList = offers.AsParallel().Select(c =>
                    {
                        mediaDict.TryGetValue(c.MediaId ?? string.Empty, out var media);

                        string? image = null;

                        if (media != null)
                        {
                            var imageData = media.FlattenedData?.Length > 0
                                ? media.FlattenedData
                                : media.Data?.Length > 0
                                    ? media.Data
                                    : null;

                            if (imageData != null && !string.IsNullOrEmpty(media.ContentType))
                                image = $"data:{media.ContentType};base64,{Convert.ToBase64String(imageData)}";

                        }

                        return new OfferDTO
                        {
                            Guid = c.Guid,
                            Status = c.Status.ToString(),
                            Image = image,
                        };
                    }).ToList();

                    return new CommonResponseTemplateWithDataArrayList<OfferDTO>
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Offers Fetched Successfully!",
                        data = offerDtoList
                    };
                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplateWithDataArrayList<OfferDTO>
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
