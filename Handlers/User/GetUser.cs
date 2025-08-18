using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers.Helpers;
using MediatR;

namespace Handlers.User
{
    public class GetUser
    {
        public class Query : IRequest<CommonResponseTemplate<UserResponseDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, CommonResponseTemplate<UserResponseDto>>
        {
            public UserDataLibrary UserDL { get; }

            public Handler(UserDataLibrary userDataLibrary)
            {
                UserDL = userDataLibrary;
            }

            public async Task<CommonResponseTemplate<UserResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await UserDL.GetUser(request.Id);

                    if (user != null)
                    {
                        var model = new UserResponseDto
                        {
                            Id = user.Id,
                            MobileNumber = user.MobileNumber,
                            Email = user.Email,
                            UserTypeId = user.UserTypeId,
                            Country = user.Country,
                            CountryCode = user.CountryCode,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                            Status = user.Status,
                            CreatedAt = user.CreatedAt,
                            UserType = user.UserType.Name,
                            FullName = user.Name,
                            ProfileImageUrl = user.IndividualProfile?.ProfileImageUrl ?? user.CompanyProfile?.LogoUrl,
                            Gender = user.IndividualProfile?.Gender,
                            DateOfBirth = user.IndividualProfile?.DateOfBirth,
                            IBAN = user.IndividualProfile?.IBAN ?? user.CompanyProfile?.IBAN,
                            LogoUrl = user.CompanyProfile?.LogoUrl,
                            DesignSpeciality = user.DesignerProfile?.DesignSpeciality,
                            PortfolioLink = user.DesignerProfile?.PortfolioLink
                        };

                        return new CommonResponseTemplate<UserResponseDto>
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "User Count Fetched Successfully!",
                            data = model
                        };
                    }
                    else
                    {
                        return new CommonResponseTemplate<UserResponseDto>
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
                    return new CommonResponseTemplate<UserResponseDto>
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
