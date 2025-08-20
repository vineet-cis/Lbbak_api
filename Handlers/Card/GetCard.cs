using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;

namespace Handlers.Card
{
    public class GetCard
    {
        public class Query : IRequest<CommonResponseTemplate<CardResponseDTO>>
        {
            public string? Guid { get; set; }
        }

        public class Handler : IRequestHandler<Query, CommonResponseTemplate<CardResponseDTO>>
        {
            public CardDataLibrary CardDL { get; }

            public Handler(CardDataLibrary cardDataLibrary)
            {
                CardDL = cardDataLibrary;
            }

            public async Task<CommonResponseTemplate<CardResponseDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if(request.Guid != null)
                    {
                        var card = await CardDL.GetCardByGuid(request.Guid);

                        if (card != null)
                        {
                            var model = new CardResponseDTO
                            {
                                Guid = card.Guid,
                                Name = card.Name,
                                EventType = card.EventType,
                                CardType = card.CardType,
                                Description = card.Description,
                                Status = card.Status,
                                ProfileMediaId = card.ProfileMediaId
                            };

                            return new CommonResponseTemplate<CardResponseDTO>
                            {
                                responseCode = ResponseCode.Success.ToString(),
                                statusCode = HttpStatusCodes.OK,
                                msg = "Card Fetched Successfully!",
                                data = model
                            };
                        }
                        else
                        {
                            return new CommonResponseTemplate<CardResponseDTO>
                            {
                                responseCode = ResponseCode.NotFound.ToString(),
                                statusCode = HttpStatusCodes.OK,
                                msg = "Card Not Found",
                                data = null
                            };
                        }
                    }
                    else
                    {
                        return new CommonResponseTemplate<CardResponseDTO>
                        {
                            responseCode = ResponseCode.NotFound.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Id is null",
                            data = null
                        };
                    }
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
