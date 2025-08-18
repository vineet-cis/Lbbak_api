namespace DataCommunication
{
    public class CommonResponseTemplate<T>
    {
        public string? responseCode { get; set; }
        public int statusCode { get; set; }
        public string? msg { get; set; }
        public T? data { get; set; }
    }

    public class CommonResponseTemplate
    {
        public string? responseCode { get; set; }
        public int statusCode { get; set; }
        public string? msg { get; set; }
        public object? data { get; set; }
    }

    public class CommonResponseTemplateWithDataArrayList<T>
    {
        public string? responseCode { get; set; }
        public int statusCode { get; set; }
        public string? msg { get; set; }
        public List<T>? data { get; set; }
    }
}
