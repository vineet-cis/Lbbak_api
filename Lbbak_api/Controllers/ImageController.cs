using DataCommunication;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Lbbak_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ImageController : BaseAPIController
    {
        private readonly IMongoCollection<MediaFile> _mediaCollection;

        public ImageController(IMongoClient client)
        {
            var db = client.GetDatabase("MediaStorage");
            _mediaCollection = db.GetCollection<MediaFile>("media");
        }

        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var media = await _mediaCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

            if (media == null || media.Data == null)
                return NotFound();

            return File(media.Data, media.ContentType ?? "");
        }

        [HttpGet("GetFlattenedImage")]
        public async Task<IActionResult> GetFlattenedImage(string id)
        {
            var media = await _mediaCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

            if (media == null)
                return NotFound();
            else if(media.FlattenedData == null)
                return File(media.Data, media.ContentType ?? "");

            return File(media.FlattenedData, media.ContentType ?? "");
        }

        [HttpGet("GetImageWithAnnotations")]
        public async Task<IActionResult> GetImageWithAnnotations(string id)
        {
            var media = await _mediaCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

            if (media == null)
                return NotFound();

            var response = new
            {
                imageUrl = $"/Image/GetImage?id={media.Id}",
                annotations = media.Annotations
            };

            return Ok(response);
        }
    }
}
