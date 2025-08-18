using DataCommunication;
using DataCommunication.DataLibraries;
using FluentValidation;
using Handlers.Helpers;
using Lbbak_api;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Handlers.Card
{
    public class UpdateCard
    {
        public class Command : IRequest<CommonResponseTemplate>
        {
            public string? Guid { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? EventType { get; set; }
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
                var card = await CardDL.GetCardByGuid(request.Guid);

                if (card == null)
                    return new CommonResponseTemplate
                    {
                        responseCode = ResponseCode.NotFound.ToString(),
                        statusCode = HttpStatusCodes.NotFound,
                        msg = "Card not found",
                        data = null
                    };

                string mediaId = card.ProfileMediaId;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var annotations = !string.IsNullOrEmpty(request.AnnotationsJson)
                    ? JsonSerializer.Deserialize<List<TextAnnotation>>(request.AnnotationsJson, options)
                    : new List<TextAnnotation>();

                if (request.formFile != null)
                {
                    if(!string.IsNullOrEmpty(mediaId))
                        await _media.DeleteAsync(mediaId);

                    mediaId = await _media.UploadAsync(request.formFile, annotations);
                    card.ProfileMediaId = mediaId; // SQL stores only ID
                }
                else if (mediaId != null && annotations != null)
                    await _media.UpdateAnnotationsAsync(mediaId, annotations);


                card.Name = request.Name ?? card.Name;
                card.Description = request.Description ?? card.Description;
                card.EventType = request.EventType ?? card.EventType;

                await CardDL.UpdateCard(card);


                return new CommonResponseTemplate
                {
                    responseCode = ResponseCode.Success.ToString(),
                    statusCode = HttpStatusCodes.OK,
                    msg = "Card Updated Successfully",
                    data = null
                };
            }
        }

    }
}
