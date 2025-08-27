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

            if (media == null || media.MediaUrl == null)
                return NotFound();

            return Ok(media.MediaUrl);
        }

        [HttpGet("GetFlattenedImage")]
        public async Task<IActionResult> GetFlattenedImage(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var media = await _mediaCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

            if (media == null || media.FlattenedImageUrl == null)
                return NotFound();

            return Ok(media.FlattenedImageUrl);
        }

        [HttpGet("GetImageWithAnnotations")]
        public async Task<IActionResult> GetImageWithAnnotations(string id)
        {
            var media = await _mediaCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

            if (media == null)
                return NotFound();

            var response = new
            {
                imageUrl = media.MediaUrl,
                annotations = media.Annotations
            };

            return Ok(response);
        }
    }
}
