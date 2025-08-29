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
        public async Task UpdateAnnotationsAsync(string? mediaId, List<TextAnnotation> annotations)
        {
            var update = Builders<MediaFile>.Update
                .Set(m => m.Annotations, annotations);

            await _mediaCollection.UpdateOneAsync(m => m.Id == mediaId, update);
        }
    }
}
