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
        public int SqlCardId { get; set; }
        public List<TextAnnotation>? Annotations { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

    public class TextAnnotation
    {
        public string? Text { get; set; }
        public double XPercent { get; set; }
        public double YPercent { get; set; }
        public float FontSize { get; set; }
        public string? fontColor { get; set; }
        public string? FontFamily { get; set; }
    }

}
