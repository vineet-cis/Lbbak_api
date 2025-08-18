using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Handlers.Helpers
{
    public static class Helper
    {
        static readonly char[] padding = { '=' };
        public static string GetGUID()
        {
            return System.Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd(padding).Replace('+', '-').Replace('/', '_').Substring(0, 8);
        }

        public static byte[] CreateThumbnail(byte[] originalImage, int width, int height)
        {
            try
            {
                using var ms = new MemoryStream(originalImage);

                var format = Image.DetectFormat(ms);
                ms.Position = 0;
                using var image = Image.Load(ms);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(width, height)
                }));

                using var outStream = new MemoryStream();
                image.Save(outStream, format);
                return outStream.ToArray();
            }
            catch(Exception ex)
            {
                return Array.Empty<byte>();
            }
        }
    }
}
