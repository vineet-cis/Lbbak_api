using DataCommunication;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using SkiaSharp;
using static Handlers.Helpers.Cloudinary;

namespace Lbbak_api
{
    public interface IMediaService
    {
        Task<string> UploadAsync(IFormFile file, string? folder, List<TextAnnotation>? annotations = null, string? mediaId = null);
        Task UpdateAsync(string mediaId, IFormFile file, List<TextAnnotation>? annotations = null);
        Task<byte[]> GetFileContentAsync(string mediaId);
        Task<bool> DeleteAsync(string mediaId);
        Task UpdateAnnotationsAsync(string mediaId, List<TextAnnotation> annotations);
    }

    public class MediaService : IMediaService
    {
        private readonly IMongoCollection<MediaFile> _mediaCollection;
        private readonly ICloudinaryService cloudi;

        public MediaService(ICloudinaryService cloudinary)
        {
            var client = new MongoClient(EnvironmentVariable.MongoConnection());
            var database = client.GetDatabase("MediaStorage");
            _mediaCollection = database.GetCollection<MediaFile>("media");
            cloudi = cloudinary;
        }

        public async Task<string> UploadAsync(IFormFile file, string? folder, List<TextAnnotation>? annotations, string? mediaId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Invalid file.");

                string mediaUrl = "";
                var MediaId = mediaId;

                if (file.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                    mediaUrl = await cloudi.UploadVideoAsync(file, folder);
                else if (file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    mediaUrl = await cloudi.UploadImageAsync(file, folder);
                else
                    throw new NotSupportedException("Unsupported file type. Only images and videos are allowed.");

                if (!string.IsNullOrEmpty(mediaId))
                {
                    var update = Builders<MediaFile>.Update
                        .Set(m => m.FileName, file.FileName)
                        .Set(m => m.ContentType, file.ContentType)
                        .Set(m => m.MediaUrl, mediaUrl)
                        .Set(m => m.Annotations, annotations);

                    await _mediaCollection.UpdateOneAsync(
                        m => m.Id == mediaId,
                        update
                    );
                }
                else
                {
                    var media = new MediaFile
                    {
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        Annotations = annotations,
                        MediaUrl = mediaUrl,
                    };

                    await _mediaCollection.InsertOneAsync(media);

                    MediaId = media.Id;
                }

                return MediaId;

            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task UpdateAsync(string mediaId, IFormFile file, List<TextAnnotation>? annotations)
        {
            if (string.IsNullOrEmpty(mediaId))
                throw new ArgumentException("Invalid mediaId.");

            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            var originalBytes = stream.ToArray();
            byte[]? flattenedBytes = null;

            if (annotations != null && annotations.Count > 0)
                flattenedBytes = GenerateFlattenedImage(originalBytes, annotations);
        }

        public async Task<byte[]> GetFileContentAsync(string mediaId)
        {
            var media = await _mediaCollection.Find(m => m.Id == mediaId).FirstOrDefaultAsync();
            return Array.Empty<byte>();
        }

        public async Task<bool> DeleteAsync(string mediaId)
        {
            var result = await _mediaCollection.DeleteOneAsync(m => m.Id == mediaId);
            return result.DeletedCount > 0;
        }

        private byte[] GenerateFlattenedImage(byte[] originalImage, List<TextAnnotation> annotations)
        {
            SKBitmap? bitmap = null;

            try
            {
                using var inputStream = new MemoryStream(originalImage);
                bitmap = SKBitmap.Decode(inputStream);
            }
            catch
            {
                bitmap = null;
            }

            int width = bitmap?.Width ?? 800;
            int height = bitmap?.Height ?? 600;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;


            if (bitmap != null)
                canvas.DrawBitmap(bitmap, 0, 0);
            else
                canvas.Clear(SKColors.White);

            annotations.ForEach(ann =>
            {

                if (!string.IsNullOrWhiteSpace(ann.Text))
                {
                    var style = SKFontStyle.Normal;
                    if (!string.IsNullOrEmpty(ann.FontWeight) || !string.IsNullOrEmpty(ann.FontStyle))
                    {
                        var weight = ann.FontWeight?.ToLower() == "bold" ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
                        var slant = ann.FontStyle?.ToLower() == "italic" ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
                        style = new SKFontStyle(weight, SKFontStyleWidth.Normal, slant);
                    }

                    using var typeface = SKTypeface.FromFamilyName(ann.FontFamily ?? "Arial", style);
                    using var font = new SKFont(typeface, ann.FontSize > 0 ? ann.FontSize : 16f);

                    using var paint = new SKPaint
                    {
                        Color = !string.IsNullOrEmpty(ann.fontColor) ? SKColor.Parse(ann.fontColor) : SKColors.Black,
                        IsAntialias = true
                    };

                    float percentX = (float)ann.XPercent / 100f;
                    float percentY = (float)ann.YPercent / 100f;

                    float x = width * percentX;
                    float y = height * percentY;

                    float textWidth = font.MeasureText(ann.Text, paint);

                    if (ann.TextAlign?.ToLower() == "center")
                        x -= textWidth / 2f;
                    else if (ann.TextAlign?.ToLower() == "right")
                        x -= textWidth;

                    if (ann.RotateDeg != 0)
                    {
                        canvas.Save();
                        canvas.Translate(x, y);
                        canvas.RotateDegrees((float)ann.RotateDeg);
                        canvas.DrawText(ann.Text, 0, 0, font, paint);
                        canvas.Restore();
                    }
                    else
                    {
                        canvas.DrawText(ann.Text, x, y, font, paint);
                    }
                }

            });

            canvas.Flush();

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }


        public async Task UpdateAnnotationsAsync(string? mediaId, List<TextAnnotation> annotations)
        {
            var update = Builders<MediaFile>.Update
                .Set(m => m.Annotations, annotations);

            await _mediaCollection.UpdateOneAsync(m => m.Id == mediaId, update);
        }
    }
}
