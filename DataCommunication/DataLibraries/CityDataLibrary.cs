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

        public async Task<List<Country>> GetCountries()
        {
            return await context.Countries.ToListAsync();
        }

        public async Task CreateCountry(string Name)
        {
            if(!string.IsNullOrWhiteSpace(Name))
            {
                var country = new Country
                {
                    Name = Name
                };

                context.Countries.Add(country);
                await context.SaveChangesAsync();
            }
        }
    }
}
