using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using Lbbak_api;
using MediatR;

namespace Handlers.Mobile.PromotionalOffer
{
    public class CreateOfferType
    {
        public class CreateCommand : IRequest<CommonResponseTemplate>
        {
            public string? Name { get; set; }
        }

        public class CommandValidator : AbstractValidator<CreateCommand>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<CreateCommand, CommonResponseTemplate>
        {
            private readonly IMediaService _media;
            private readonly OfferDataLibrary OfferDL;

            public Handler(OfferDataLibrary offerDataLibrary, IMediaService mediaService)
            {
                _media = mediaService;
                OfferDL = offerDataLibrary;
            }
            public async Task<CommonResponseTemplate> Handle(CreateCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    await OfferDL.CreateOfferType(new DataCommunication.OfferCategory
                    {
                        Name = request.Name
                    });

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
