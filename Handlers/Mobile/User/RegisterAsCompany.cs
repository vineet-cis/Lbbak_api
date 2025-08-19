using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using MediatR;

namespace Handlers.Mobile.User
{
    public class RegisterAsCompany
    {
        public class RegisterAsCompanyCommand : IRequest<CommonResponseTemplate>
        {
            public string? CountryCode { get; set; }
            public string? Country { get; set; }
            public string? MobileNumber { get; set; }
            public string? CommercialRegistrationNumber { get; set; }
        }

        public class CommandValidator : AbstractValidator<RegisterAsCompanyCommand>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<RegisterAsCompanyCommand, CommonResponseTemplate>
        {
            private readonly UserDataLibrary UserDL;

            public Handler(UserDataLibrary userDataLibrary)
            {
                UserDL = userDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(RegisterAsCompanyCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var userId = await UserDL.GetUserByCodeAndNumber(request.CountryCode, request.MobileNumber);
                    if (!string.IsNullOrEmpty(userId))
                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.BadRequest.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "User Already Exists with Mobile Number",
                            data = userId
                        };


                    var user = await UserDL.CreateUser(new DataCommunication.User
                    {
                        Id = Guid.NewGuid(),
                        MobileNumber = request.MobileNumber,
                        UserTypeId = 2, // Company
                        CountryCode = request.CountryCode,
                        Country = request.Country,
                        TwoFactorEnabled = true,
                    });

                    var companyProfile = await UserDL.CreateCompanyUser(new CompanyUser
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        CommercialRegistrationNumber = request.CommercialRegistrationNumber
                    });

                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "User Created Successfully!",
                        data = user.Id.ToString()
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
