namespace DataCommunication.DTOs
{
    public class CardResponseDTO
    {
        public string? Name { get; set; }
        public string? EventType { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? ProfileMediaId { get; set; }
        public string? Guid { get; set; }
        public string? ImageUrl { get; set; }
        public List<TextAnnotation>? Annotations { get; set; }
        public string? ThumbnailBase64 { get; set; }
    }
}
