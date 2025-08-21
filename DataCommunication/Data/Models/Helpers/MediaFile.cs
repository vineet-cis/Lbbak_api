using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataCommunication
{
    public class MediaFile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }  // Use string for easier JSON serialization
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? Data { get; set; }
        public byte[]? FlattenedData { get; set; }
        public string? SqlUserId { get; set; }
        public int? SqlCardId { get; set; }
        public int? SqlEventId { get; set; }
        public List<TextAnnotation>? Annotations { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

    public class TextAnnotation
    {
        // Content
        public string? Text { get; set; }

        // Position (percent-based)
        public double XPercent { get; set; }
        public double YPercent { get; set; }

        // Dimensions
        public int Width { get; set; }
        public int Height { get; set; }

        // Styling
        public float FontSize { get; set; }
        public string? FontColor { get; set; }
        public string? FontFamily { get; set; }
        public string? FontWeight { get; set; }
        public string? FontStyle { get; set; }
        public string? TextAlign { get; set; }
        public double RotateDeg { get; set; }
    }

}
