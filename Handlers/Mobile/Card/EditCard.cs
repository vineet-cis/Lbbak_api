using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using FluentValidation;
using Handlers.Helpers;
using Lbbak_api;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System.Text.Json;

namespace Handlers
{
    public class EditCard
    {
        public class EditCardCommand : IRequest<CommonResponseTemplate>
        {
            public string CardGuid { get; set; }
            public string EventGuid { get; set; }
            public string? AnnotationsJson { get; set; }
            public IFormFile? formFile { get; set; }
        }

        public class CommandValidator : AbstractValidator<EditCardCommand>
        {
            public CommandValidator()
            {
            }
        }

        public class Handler : IRequestHandler<EditCardCommand, CommonResponseTemplate>
        {
            private readonly IMediaService _media;
            private readonly CardDataLibrary CardDL;
            private readonly EventDataLibrary EventDL;

            public Handler(IMediaService mediaService, CardDataLibrary cardDataLibrary, EventDataLibrary eventDataLibrary)
            {
                _media = mediaService;
                CardDL = cardDataLibrary;
                EventDL = eventDataLibrary;
            }
            public async Task<CommonResponseTemplate> Handle(EditCardCommand request, CancellationToken cancellationToken)
            {
                bool isVideo = false;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var annotations = !string.IsNullOrEmpty(request.AnnotationsJson)
                    ? JsonSerializer.Deserialize<List<TextAnnotation>>(request.AnnotationsJson, options)
                    : new List<TextAnnotation>();

                string mediaId = "";

                if (request.formFile != null && request.formFile.Length > 0)
                {
                    mediaId = await _media.UploadAsync(request.formFile, annotations);

                    var extension = Path.GetExtension(request.formFile.FileName).ToLower();

                    string[] videoExtensions = { ".mp4", ".avi", ".mov", ".mkv" };
                    isVideo = videoExtensions.Contains(extension);
                }

                await CardDL.AddCardUseCount(request.CardGuid);
                await EventDL.AddEventMedia(request.EventGuid, mediaId, isVideo);

                return new CommonResponseTemplate
                {
                    responseCode = ResponseCode.Success.ToString(),
                    statusCode = HttpStatusCodes.OK,
                    msg = "Card Edited Successfully",
                    data = null
                };
            }
        }
    }
}
