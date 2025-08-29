using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers.Helpers;
using MediatR;
using static DataCommunication.CommonComponents.Enums;

namespace Handlers
{
    public class EditAdmin
    {
        public class EditAdminCommand : IRequest<CommonResponseTemplate>
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? MobileNumber { get; set; }
            public string? Email { get; set; }
            public int Status { get; set; }
            public string? Password { get; set; }
            public string[]? Permissions { get; set; }
            public string[]? Countries { get; set; }
        }

        public class Handler : IRequestHandler<EditAdminCommand, CommonResponseTemplate>
        {
            private readonly AdminDataLibrary AdminDL;

            public Handler(AdminDataLibrary adminDataLibrary)
            {
                AdminDL = adminDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(EditAdminCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var admin = await AdminDL.getAdmin(request.Id);

                    if (admin != null)
                    {
                        if (!string.IsNullOrEmpty(request.Name)) admin.FullName = request.Name;
                        if (!string.IsNullOrEmpty(request.MobileNumber)) admin.Mobile = request.MobileNumber;
                        if (!string.IsNullOrEmpty(request.Email)) admin.Email = request.Email;

                        admin.Status = (Status)request.Status;

                        if (!string.IsNullOrEmpty(request.Password))
                            if (!BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
                                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                        admin.Permissions = request.Permissions ?? admin.Permissions;
                        admin.Countries = request.Countries ?? admin.Countries;

                        await AdminDL.UpdateAdmin(admin);

                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Admin Updated Successfully!",
                            data = null
                        };
                    }
                    else
                    {
                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.NotFound.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Admin Not Found!",
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
