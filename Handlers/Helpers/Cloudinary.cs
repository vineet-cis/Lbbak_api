using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using SkiaSharp;

namespace Handlers.Helpers
{
    public class Cloudinary
    {
        public interface ICloudinaryService
        {
            Task<string> UploadImageAsync(IFormFile file, string? folder, List<DataCommunication.TextAnnotation>? annotations = null);
            Task<string> UploadVideoAsync(IFormFile file, string folder);
        }

        public class CloudinaryService : ICloudinaryService
        {
            private readonly CloudinaryDotNet.Cloudinary _cloudinary;

            public CloudinaryService()
            {
                var account = new Account(
                    "dmtqgjmkx",
                    "284539653471311",
                    "bJxCFDEFd_-LTlB4U5Tl09UIJIQ");

                _cloudinary = new CloudinaryDotNet.Cloudinary(account);
            }

            public async Task<string> UploadImageAsync(IFormFile file, string? folder, List<DataCommunication.TextAnnotation>? annotations = null)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var originalBytes = ms.ToArray();

                byte[] imageToUpload;

                if (annotations != null && annotations.Any())
                    imageToUpload = GenerateFlattenedImage(originalBytes, annotations);
                else
                    imageToUpload = originalBytes;

                using var uploadStream = new MemoryStream(imageToUpload);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, uploadStream),
                    Folder = folder,
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl?.ToString();
            }


            public async Task<string> UploadVideoAsync(IFormFile file, string folder)
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new VideoUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl?.ToString();
            }
        }

        private static byte[] GenerateFlattenedImage(byte[] originalImage, List<DataCommunication.TextAnnotation> annotations)
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

            annotations.ForEach(ann => {

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
    }
}
