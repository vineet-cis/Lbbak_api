using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers.Helpers;
using MediatR;

namespace Handlers
{
    public class EditPermissions
    {
        public class EditCommand : IRequest<CommonResponseTemplate>
        {
            public Guid Id { get; set; }
            public string[]? Permissions { get; set; }
            public string[]? Countries { get; set; }
        }

        public class Handler : IRequestHandler<EditCommand, CommonResponseTemplate>
        {
            private readonly AdminDataLibrary AdminDL;

            public Handler(AdminDataLibrary adminDataLibrary)
            {
                AdminDL = adminDataLibrary;
            }

            public async Task<CommonResponseTemplate> Handle(EditCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var admin = await AdminDL.getAdmin(request.Id);

                    if (admin != null)
                    {
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
