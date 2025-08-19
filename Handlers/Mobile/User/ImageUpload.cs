using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using Lbbak_api;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Handlers.Mobile.User
{
    public class ImageUpload
    {
        public class Command : IRequest<CommonResponseTemplate>
        {
            public Guid Id { get; set; }
            public IFormFile? formFile { get; set; }
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
            private readonly IMediaService _media;

            public Handler(UserDataLibrary userDataLibrary, IMediaService mediaService)
            {
                UserDL = userDataLibrary;
                _media = mediaService;
            }

            public async Task<CommonResponseTemplate> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await UserDL.GetUserOnly(request.Id);

                    if (user != null)
                    {
                        var mediaId = "";
                        if(request.formFile != null && request.formFile.Length > 0)
                        {
                            mediaId = await _media.UploadAsync(request.formFile, null, user.Id.ToString());
                            user.ProfileMediaId = mediaId;
                            await UserDL.UpdateUser(user);
                        }
                        else
                            return new CommonResponseTemplate
                            {
                                responseCode = ResponseCode.Empty.ToString(),
                                statusCode = HttpStatusCodes.InvalidInput,
                                msg = "Invalid Image!",
                                data = null
                            };

                        return new CommonResponseTemplate
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Image Uploaded Successfully!",
                            data = $"/Image/GetImage?id={mediaId}"
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
