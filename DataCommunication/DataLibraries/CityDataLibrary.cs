using Microsoft.EntityFrameworkCore;

namespace DataCommunication.DataLibraries
{
    public class CityDataLibrary
    {
        private readonly AppDbContext context;

        public CityDataLibrary(AppDbContext appDbContext)
        {
            context = appDbContext;
        }

        public async Task<List<City>> GetAllCities()
        {
            return await context.City.ToListAsync();
        }
    }
}
