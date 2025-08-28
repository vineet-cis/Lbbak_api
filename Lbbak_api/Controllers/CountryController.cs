using DataCommunication;
using DataCommunication.DataLibraries;
using Microsoft.AspNetCore.Mvc;

namespace Lbbak_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : Controller
    {
        private readonly CityDataLibrary db;

        public CountryController(CityDataLibrary cityDataLibrary) 
        { 
            db = cityDataLibrary;
        }

        [HttpPost("CreateCountry")]
        public async Task<IActionResult> CreateAdmin([FromBody] string Name)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await db.CreateCountry(Name);

            return Ok(new
            {
                Message = "Country created successfully."
            });
        }

        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countries =  await db.GetCountries();

            if(countries.Count == 0)
                return NotFound("No Countries Found");

            return Ok(new
            {
                Countries = countries,
                Message = "Country created successfully."
            });
        }
    }
}
