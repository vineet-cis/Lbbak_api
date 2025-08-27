using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using Lbbak_api;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using static DataCommunication.CommonComponents.Enums;

namespace Handlers.Card
{
    public class CreateCard
    {
        public class Command : IRequest<CommonResponseTemplate>
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public int CardType { get; set; }
            public string? AnnotationsJson { get; set; }
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
            private readonly IMediaService _media;
            private readonly CardDataLibrary CardDL;

            public Handler(IMediaService mediaService, CardDataLibrary cardDataLibrary)
            {
                _media = mediaService;
                CardDL = cardDataLibrary;
            }
            public async Task<CommonResponseTemplate> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var imageId = "";

                    if (request.formFile != null && request.formFile.Length > 0)
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        var annotations = !string.IsNullOrEmpty(request.AnnotationsJson)
                            ? JsonSerializer.Deserialize<List<TextAnnotation>>(request.AnnotationsJson, options)
                            : new List<TextAnnotation>();

                        imageId = await _media.UploadAsync(request.formFile, "Card", annotations);
                    }

                    int cardId = await CardDL.CreateCard(new DataCommunication.Card
                    {
                        Name = request.Name,
                        Description = request.Description,
                        CardType = (CardType)request.CardType,
                        ProfileMediaId = imageId,
                        Guid = Helper.GetGUID()
                    });

                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.Success.ToString(),
                        statusCode = HttpStatusCodes.OK,
                        msg = "Card Created Successfully",
                        data = null
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
