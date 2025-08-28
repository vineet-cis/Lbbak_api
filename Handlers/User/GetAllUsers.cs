using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers.Helpers;
using MediatR;
using MongoDB.Driver;

namespace Handlers.User
{
    public class GetAllUsers
    {
        public class Query : IRequest<CommonResponseTemplateWithDataArrayList<UserResponseDto>>
        { }

        public class Handler : IRequestHandler<Query, CommonResponseTemplateWithDataArrayList<UserResponseDto>>
        {
            public UserDataLibrary UserDL { get; }

            private readonly IMapper _mapper;
            private readonly IMongoCollection<MediaFile> _mediaCollection;

            public Handler(UserDataLibrary userDataLibrary, IMapper mapper, IMongoClient client)
            {
                UserDL = userDataLibrary;
                _mapper = mapper;
                var db = client.GetDatabase("MediaStorage");
                _mediaCollection = db.GetCollection<MediaFile>("media");
            }

            public async Task<CommonResponseTemplateWithDataArrayList<UserResponseDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var users = await UserDL.GetAllUsers();

                    if(users.Count > 0)
                    {

                        var mediaIds = users.Select(c => c.ProfileMediaId)
                        .Where(id => !string.IsNullOrEmpty(id))
                        .Distinct()
                        .ToList();

                        var mediaList = await _mediaCollection.Find(m => mediaIds.Contains(m.Id)).ToListAsync();

                        var mediaDict = mediaList
                            .Where(m => !string.IsNullOrEmpty(m.Id))
                            .ToDictionary(m => m.Id!, m => m);

                        var userDtoList = users.Select(u =>
                        {
                            var nameParts = (u.Name ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                            var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

                            mediaDict.TryGetValue(u.ProfileMediaId ?? string.Empty, out var media);

                            return new UserResponseDto
                            {
                                Id = u.Id,
                                MobileNumber = u.MobileNumber,
                                Email = u.Email,
                                UserTypeId = u.UserTypeId,
                                Country = u.Country,
                                CountryCode = u.CountryCode,
                                TwoFactorEnabled = u.TwoFactorEnabled,
                                CommercialRegistrationNumber = u.CompanyProfile?.CommercialRegistrationNumber ?? null,
                                Age = u.IndividualProfile != null ? u.IndividualProfile.Age : u.DesignerProfile != null ? u.DesignerProfile.Age : 0,
                                Status = u.Status,
                                CreatedAt = u.CreatedAt,
                                UserType = u.UserType.Name,
                                FullName = u.Name,
                                FirstName = firstName,
                                LastName = lastName,
                                ProfileImageUrl = media?.MediaUrl ?? null,
                                Gender = u.IndividualProfile?.Gender,
                                DateOfBirth = u.IndividualProfile?.DateOfBirth,
                                IBAN = u.IndividualProfile?.IBAN ?? u.CompanyProfile?.IBAN
                            };
                        }).ToList();


                        return new CommonResponseTemplateWithDataArrayList<UserResponseDto>
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "User Count Fetched Successfully!",
                            data = _mapper.Map<List<UserResponseDto>, List<UserResponseDto>>(userDtoList)
                        };
                    }
                    else
                    {
                        return new CommonResponseTemplateWithDataArrayList<UserResponseDto>
                        {
                            responseCode = ResponseCode.Empty.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "No Users Found!",
                            data = null
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplateWithDataArrayList<UserResponseDto>
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
