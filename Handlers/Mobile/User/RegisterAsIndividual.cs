using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using MediatR;

namespace Handlers.Mobile.User
{
    public class RegisterAsIndividual
    {
        public class RegisterAsIndividualCommand: IRequest<CommonResponseTemplate>
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? MobileNumber { get; set; }
            public string? CountryCode { get; set; }
            public string? Country { get; set; }
            public string? Email { get; set; }
            public string? Gender { get; set; }
            public DateTime? DateOfBirth { get; set; }
        }

        public class CommandValidator : AbstractValidator<RegisterAsIndividualCommand>
        {
            public CommandValidator()
            {
                RuleFor(x => x.DateOfBirth).NotNull().LessThan(DateTime.Now);
            }
        }

        public class Handler : IRequestHandler<RegisterAsIndividualCommand, CommonResponseTemplate>
        {
            private readonly UserDataLibrary UserDL;

            public Handler(UserDataLibrary userDataLibrary)
            {
                UserDL = userDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(RegisterAsIndividualCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await UserDL.CreateUser(new DataCommunication.User
                    {
                        Id = Guid.NewGuid(),
                        Email = request.Email,
                        Name = request.FirstName + " " + request.LastName,
                        UserTypeId = 1, // Individual,
                        CountryCode = request.CountryCode,
                        Country = request.Country,
                        MobileNumber = request.MobileNumber,
                        TwoFactorEnabled = true
                    });

                     var individualProfile = await UserDL.CreateIndividual(new IndividualUser
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        DateOfBirth = request.DateOfBirth.Value,
                        Gender = request.Gender
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
