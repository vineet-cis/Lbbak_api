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
    public class UpdateOffer
    {
        public class UpdateOfferCommand : IRequest<CommonResponseTemplate>
        {
            public IFormFile? formFile { get; set; }
            public string Guid { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? UserId { get; set; }
            public string? Link { get; set; }
            public string? LocationLink { get; set; }
            public int? Type { get; set; }
            public int? Category { get; set; }
            public int? Scope { get; set; }
            public int? City { get; set; }
            public int? Status { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class CommandValidator : AbstractValidator<UpdateOfferCommand>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<UpdateOfferCommand, CommonResponseTemplate>
        {
            private readonly IMediaService _media;
            private readonly OfferDataLibrary OfferDL;

            public Handler(OfferDataLibrary offerDataLibrary, IMediaService mediaService)
            {
                _media = mediaService;
                OfferDL = offerDataLibrary;
            }
            public async Task<CommonResponseTemplate> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var offer = await OfferDL.GetOffer(request.Guid);

                    if (offer == null)
                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.NotFound.ToString(),
                            statusCode = HttpStatusCodes.NotFound,
                            msg = "Offer not found",
                            data = null
                        };

                    var mediaId = offer.MediaId;

                    if (request.formFile != null && request.formFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(mediaId))
                        {
                            await _media.UpdateAsync(mediaId, request.formFile);
                        }
                        else
                        {
                            mediaId = await _media.UploadAsync(request.formFile, null, null, null, offer.Id);
                            offer.MediaId = mediaId;
                        }
                    }

                    if (!string.IsNullOrEmpty(request.Title)) offer.Title = request.Title;
                    if (!string.IsNullOrEmpty(request.Description)) offer.Description = request.Description;
                    if (!string.IsNullOrEmpty(request.Link)) offer.Link = request.Link;
                    if (!string.IsNullOrEmpty(request.LocationLink)) offer.LocationLink = request.LocationLink;

                    if (request.Type != null)
                        offer.Type = (PromotionType)request.Type;

                    if (request.Scope != null)
                        offer.Scope = (PromotionScope)request.Scope;

                    if (request.Type != null)
                        offer.Type = (PromotionType)request.Type;

                    if (request.Status != null)
                        offer.Status = (Status)request.Status;

                    if (request.StartDate.HasValue) offer.StartDate = request.StartDate.Value;
                    if (request.EndDate.HasValue) offer.EndDate = request.EndDate.Value;

                    await OfferDL.UpdateOffer(offer);

                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Offer Updated Successfully!",
                        data = null
                    };
                }
                catch(Exception ex)
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
