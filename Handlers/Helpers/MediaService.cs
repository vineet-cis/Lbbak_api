using DataCommunication;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using SkiaSharp;

namespace Lbbak_api
{
    public interface IMediaService
    {
        Task<string> UploadAsync(IFormFile file, int? cardId, string? userId, int? eventId);
        Task<string> UploadAsync(IFormFile file, List<TextAnnotation>? annotations = null);
        Task<bool> UpdateCardIdAsync(string mediaId, int newCardId);
        Task<bool> UpdateEventIdAsync(string mediaId, int newEventId);
        Task<byte[]> GetFileContentAsync(string mediaId);
        Task<bool> DeleteAsync(string mediaId);
        Task UpdateAnnotationsAsync(string mediaId, List<TextAnnotation> annotations);
    }

    public class MediaService : IMediaService
    {
        private readonly IMongoCollection<MediaFile> _mediaCollection;

        public MediaService()
        {
            var client = new MongoClient(EnvironmentVariable.MongoConnection());
            var database = client.GetDatabase("MediaStorage"); // or parse from conn string
            _mediaCollection = database.GetCollection<MediaFile>("media");
        }

        public async Task<string> UploadAsync(IFormFile file, int? cardId, string? userId, int? eventId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            var media = new MediaFile
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Data = stream.ToArray(),
                SqlUserId = userId,
                SqlCardId = cardId,
                SqlEventId = eventId
            };

            await _mediaCollection.InsertOneAsync(media);
            return media.Id;
        }

        public async Task<string> UploadAsync(IFormFile file, List<TextAnnotation>? annotations)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Invalid file.");

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                var originalBytes = stream.ToArray();
                byte[]? flattenedBytes = null;

                if (annotations != null && annotations.Count > 0)
                    flattenedBytes = GenerateFlattenedImage(originalBytes, annotations);

                var media = new MediaFile
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Data = originalBytes,
                    FlattenedData = flattenedBytes,
                    Annotations = annotations
                };

                await _mediaCollection.InsertOneAsync(media);
                return media.Id;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task<byte[]> GetFileContentAsync(string mediaId)
        {
            var media = await _mediaCollection.Find(m => m.Id == mediaId).FirstOrDefaultAsync();
            return media?.Data;
        }

        public async Task<bool> DeleteAsync(string mediaId)
        {
            var result = await _mediaCollection.DeleteOneAsync(m => m.Id == mediaId);
            return result.DeletedCount > 0;
        }

        public async Task<bool> UpdateCardIdAsync(string mediaId, int newCardId)
        {
            var filter = Builders<MediaFile>.Filter.Eq(m => m.Id, mediaId);
            var update = Builders<MediaFile>.Update.Set(m => m.SqlCardId, newCardId);

            var result = await _mediaCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateEventIdAsync(string mediaId, int newEventId)
        {
            var filter = Builders<MediaFile>.Filter.Eq(m => m.Id, mediaId);
            var update = Builders<MediaFile>.Update.Set(m => m.SqlEventId, newEventId);

            var result = await _mediaCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        private byte[] GenerateFlattenedImage(byte[] originalImage, List<TextAnnotation> annotations)
        {
            using var inputStream = new MemoryStream(originalImage);
            using var bitmap = SKBitmap.Decode(inputStream);

            if (bitmap != null)
            {
                using var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height));
                var canvas = surface.Canvas;

                // Draw original
                canvas.DrawBitmap(bitmap, 0, 0);

                // Draw annotations
                foreach (var ann in annotations)
                {
                    var typeface = SKTypeface.FromFamilyName(ann.FontFamily ?? "Arial");

                    using var font = new SKFont(typeface, ann.FontSize);
                    using var paint = new SKPaint
                    {
                        Color = SKColor.Parse(ann.fontColor),
                        IsAntialias = true
                    };

                    float x = (float)(bitmap.Width * ann.XPercent / 100.0f);
                    float y = (float)(bitmap.Height * ann.YPercent / 100.0f);

                    if (!string.IsNullOrWhiteSpace(ann.Text))
                    {
                        using var blob = SKTextBlob.Create(ann.Text, font);
                        canvas.DrawText(blob, x, y, paint);
                    }

                }

                canvas.Flush();

                using var image = surface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                return data.ToArray();
            }
            else
                return Array.Empty<byte>();
            
        }

        public async Task UpdateAnnotationsAsync(string? mediaId, List<TextAnnotation> annotations)
        {
            var update = Builders<MediaFile>.Update
                .Set(m => m.Annotations, annotations);

            await _mediaCollection.UpdateOneAsync(m => m.Id == mediaId, update);
        }
    }
}
