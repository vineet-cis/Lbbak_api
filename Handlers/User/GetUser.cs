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
                        var nameParts = (user.Name ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                        var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

                        var model = new UserResponseDto
                        {
                            Id = user.Id,
                            MobileNumber = user.MobileNumber,
                            Email = user.Email,
                            UserTypeId = user.UserTypeId,
                            Country = user.Country,
                            CountryCode = user.CountryCode,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                            CommercialRegistrationNumber = user.CompanyProfile?.CommercialRegistrationNumber ?? null,
                            Status = user.Status,
                            CreatedAt = user.CreatedAt,
                            UserType = user.UserType.Name,
                            FullName = user.Name,
                            FirstName = firstName,
                            LastName = lastName,
                            ProfileImageUrl = user.ProfileMediaId,
                            Gender = user.IndividualProfile?.Gender,
                            DateOfBirth = user.IndividualProfile?.DateOfBirth,
                            IBAN = user.IndividualProfile?.IBAN ?? user.CompanyProfile?.IBAN,
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
