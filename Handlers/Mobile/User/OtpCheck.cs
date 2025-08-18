using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers.Helpers;
using MediatR;

namespace Handlers.Mobile.User
{
    public class OtpCheck
    {
        public class Query : IRequest<CommonResponseTemplate>
        {
            public string? Otp { get; set; }
            public string? Code { get; set; }
            public string? Number { get; set; }
        }

        public class Handler : IRequestHandler<Query, CommonResponseTemplate>
        {
            private readonly UserDataLibrary UserDL;

            public Handler(UserDataLibrary userDataLibrary)
            {
                UserDL = userDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.Otp == "123456")
                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Success!",
                            data = await UserDL.GetUserByCodeAndNumber(request.Code, request.Number)
                        };
                    else
                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.Failed.ToString(),
                            statusCode = HttpStatusCodes.ExpectationFailed,
                            msg = "Failed!",
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
