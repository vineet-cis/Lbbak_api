using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using MediatR;

namespace Handlers.User
{
    public class UpdateUserMobile
    {
        public class UpdateUserCommand : IRequest<CommonResponseTemplate>
        {
            public Guid Id { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Gender { get; set; }
            public int? City { get; set; }
            public string? MobileNumber { get; set; }
            public string? CountryCode { get; set; }
            public string? Email { get; set; }
        }

        public class CommandValidator : AbstractValidator<UpdateUserCommand>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, CommonResponseTemplate>
        {
            private readonly UserDataLibrary UserDL;

            public Handler(UserDataLibrary userDataLibrary)
            {
                UserDL = userDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await UserDL.GetUser(request.Id);

                    if (user != null)
                    {
                        user.Name = request.FirstName + " " + request.LastName;
                        user.MobileNumber = request.MobileNumber;
                        user.Email = request.Email;
                        user.CountryCode = request.CountryCode;
                        user.CityId = request.City;

                        if (user.IndividualProfile != null)
                            user.IndividualProfile.Gender = request.Gender;

                        await UserDL.UpdateUser(user);

                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "User Updated Successfully!",
                            data = null
                        };
                    }
                    else
                    {
                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.NotFound.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "User Not Found!",
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
