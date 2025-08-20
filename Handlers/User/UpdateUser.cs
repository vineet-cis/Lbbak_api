using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using MediatR;

namespace Handlers.User
{
    public class UpdateUser
    {
        public class Command : IRequest<CommonResponseTemplate>
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? Status { get; set; }
            public string? Gender { get; set; }
            public string? Region { get; set; }
            public string? MobileNumber { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
            public string? IBAN { get; set; }
            public string? BankName { get; set; }
            public int Age { get; set; }
            public DateTime RegisterDate { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<Command, CommonResponseTemplate>
        {
            private readonly UserDataLibrary UserDL;

            public Handler(UserDataLibrary userDataLibrary)
            {
                UserDL = userDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await UserDL.GetUser(request.Id);

                    if (user != null)
                    {
                        user.Name = request.Name;
                        user.MobileNumber = request.MobileNumber;
                        user.Email = request.Email;
                        user.Status = request.Status;
                        user.CreatedAt = request.RegisterDate;

                        if (user.IndividualProfile != null)
                        {
                            user.IndividualProfile.IBAN = request.IBAN;
                            user.IndividualProfile.BankName = request.BankName;
                            user.IndividualProfile.Gender = request.Gender;
                        }
                        else if (user.CompanyProfile != null)
                        {
                            user.CompanyProfile.IBAN = request.IBAN;
                            user.CompanyProfile.BankName = request.BankName;
                        }
                        else if (user.DesignerProfile != null)
                        {
                            user.DesignerProfile.Gender = request.Gender;
                        }

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
