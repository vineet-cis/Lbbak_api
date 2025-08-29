using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers.Helpers;
using MediatR;

namespace Handlers
{
    public class DeleteOfferType
    {
        public class Command : IRequest<CommonResponseTemplate>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, CommonResponseTemplate>
        {
            private readonly OfferDataLibrary OfferDL;

            public Handler(OfferDataLibrary offerDataLibrary)
            {
                OfferDL = offerDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    await OfferDL.DeleteCategory(request.Id);

                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Category Removed",
                        data = null
                    };
                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplate
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
