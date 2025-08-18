using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;

namespace Handlers.User
{
    public class GetUserCount
    {
        public class Query : IRequest<CommonResponseTemplate<UsersCountDTO>>
        { }

        public class Handler : IRequestHandler<Query, CommonResponseTemplate<UsersCountDTO>>
        {
            public UserDataLibrary UserDL { get; }
            public Handler(UserDataLibrary userDataLibrary)
            {
                UserDL = userDataLibrary;
            }

            public async Task<CommonResponseTemplate<UsersCountDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var total = await UserDL.GetTotalUsers();
                    var individual = await UserDL.GetTotalIndividualUsers();
                    var company = await UserDL.GetCompanyUsers();
                    var designer = await UserDL.GetDesignerUsers();
                    var blocked = await UserDL.GetBlockedUsers();

                    return new CommonResponseTemplate<UsersCountDTO>
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "User Count Fetched Successfully!",
                        data = new UsersCountDTO
                        {
                            total = total,
                            individual = individual,
                            company = company,
                            designer = designer,
                            blocked = blocked
                        }
                    };
                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplate<UsersCountDTO>
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
