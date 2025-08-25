using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers.Helpers;
using MediatR;

namespace Handlers
{
    public class GetUserByNumber
    {
        public class Query : IRequest<CommonResponseTemplate>
        {
            public string? Code { get; set; }
            public string? Number { get; set; }
        }

        public class Handler : IRequestHandler<Query, CommonResponseTemplate>
        {
            public UserDataLibrary UserDL { get; }

            public Handler(UserDataLibrary userDataLibrary)
            {
                UserDL = userDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var userId = await UserDL.GetUserByCodeAndNumber(request.Code, request.Number);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "User Exists!",
                            data = userId
                        };
                    }
                    else
                    {
                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.NotFound.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "User Not Found",
                            data = null
                        };
                    }
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
