using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using Lbbak_api;
using MediatR;

namespace Handlers.Card
{
    public class DeleteCard
    {
        public class Command : IRequest<CommonResponseTemplate>
        {
            public string? Guid { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<Command, CommonResponseTemplate>
        {
            private readonly IMediaService _media;
            private readonly CardDataLibrary CardDL;

            public Handler(IMediaService mediaService, CardDataLibrary cardDataLibrary)
            {
                _media = mediaService;
                CardDL = cardDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var card = await CardDL.GetCardByGuid(request.Guid);

                    if (card == null)
                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.NotFound.ToString(),
                            statusCode = HttpStatusCodes.NotFound,
                            msg = "Card Not Found",
                            data = null
                        };

                    card.Status = "Deleted";

                    await CardDL.UpdateCard(card);

                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Card Deleted Successfully",
                        data = null
                    };
                }
                catch (Exception ex)
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
