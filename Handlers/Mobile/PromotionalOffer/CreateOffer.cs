using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using Lbbak_api;
using MediatR;
using Microsoft.AspNetCore.Http;
using static DataCommunication.CommonComponents.Enums;

namespace Handlers
{
    public class CreateOffer
    {
        public class CreateOfferCommand : IRequest<CommonResponseTemplate>
        {
            public IFormFile formFile { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? UserId { get; set; }
            public string? Link { get; set; }
            public string? LocationLink { get; set; }
            public int Type { get; set; }
            public int Category { get; set; }
            public int Scope { get; set; }
            public int City { get; set; }
            public int Status { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public class CommandValidator : AbstractValidator<CreateOfferCommand>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<CreateOfferCommand, CommonResponseTemplate>
        {
            private readonly IMediaService _media;
            private readonly OfferDataLibrary OfferDL;

            public Handler(OfferDataLibrary offerDataLibrary, IMediaService mediaService)
            {
                _media = mediaService;
                OfferDL = offerDataLibrary;
            }
            public async Task<CommonResponseTemplate> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var offer = await OfferDL.Create(new DataCommunication.PromotionalOffer
                    {
                        Guid = Helper.GetGUID(),
                        CreatedBy = request.UserId,
                        UpdatedBy = request.UserId,
                        Title = request.Title,
                        Description = request.Description,
                        Link = request.Link,
                        LocationLink = request.LocationLink,
                        Type = (PromotionType)request.Type,
                        Scope = (PromotionScope)request.Scope,
                        Status = (Status)request.Status,
                        CategoryId = request.Category,
                        CityId = request.City,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate
                    });

                    var mediaId = "";

                    if (request.formFile != null && request.formFile.Length > 0)
                        mediaId = await _media.UploadAsync(request.formFile, null, null, null, offer.Id);

                    await OfferDL.UpdateOfferMediaId(offer, mediaId);

                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Offer Created Successfully!",
                        data = null
                    };
                }
                catch
                (Exception ex)
                {
                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.InternalServerError.ToString(),
                        statusCode = HttpStatusCodes.InternalServerError,
                        msg = ex.Message,
                        data = null
                    };
                }
            }
        }
    }
}
