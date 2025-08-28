using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers.Helpers;
using MediatR;
using MongoDB.Driver;

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
            private readonly IMongoCollection<MediaFile> _mediaCollection;

            public UserDataLibrary UserDL { get; }

            public Handler(UserDataLibrary userDataLibrary, IMongoClient client)
            {
                UserDL = userDataLibrary;
                var db = client.GetDatabase("MediaStorage");
                _mediaCollection = db.GetCollection<MediaFile>("media");
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

                        var media = await _mediaCollection.Find(m => m.Id == user.ProfileMediaId).FirstOrDefaultAsync();

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
                            Age = user.IndividualProfile != null ? user.IndividualProfile.Age : user.DesignerProfile != null ? user.DesignerProfile.Age : 0,
                            CreatedAt = user.CreatedAt,
                            UserType = user.UserType.Name,
                            FullName = user.Name,
                            FirstName = firstName,
                            LastName = lastName,
                            ProfileImageUrl = media?.MediaUrl ?? null,
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
