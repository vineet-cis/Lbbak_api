using Microsoft.EntityFrameworkCore;
using static DataCommunication.CommonComponents.Enums;

namespace DataCommunication.DataLibraries
{
    public class AdminDataLibrary
    {
        private readonly AppDbContext context;

        public AdminDataLibrary(AppDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task<Admin> GetAdmin(Guid Id)
        {
            return await context.Admins
                .FirstOrDefaultAsync(a => a.Id == Id && a.Status == Status.Active);
        }

        public async Task<Admin> GetAdminByEmail(string email)
        {
            return await context.Admins
                .FirstOrDefaultAsync(a => a.Email == email && a.Status == Status.Active);
        }

        public async Task<RefreshToken> GetStoredToken(Guid adminId, string token)
        {
            return await context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.AdminId == adminId && rt.Token == token && !rt.IsRevoked);
        }

        public async Task<RefreshToken> ValidToken(Guid id)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(x => x.AdminId == id && DateTime.UtcNow <= x.ExpiryDate);
        }

        public async Task RevokToken(RefreshToken refreshToken)
        {
            context.ChangeTracker.Clear();
            refreshToken.IsRevoked = true;
            context.Entry(refreshToken).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task UpdateAdmin(Admin admin)
        {
            context.ChangeTracker.Clear();
            context.Entry(admin).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task<Admin> getAdmin(Guid guid)
        {
            return await context.Admins.FirstOrDefaultAsync(x => x.Id == guid);
        }

        public async Task CreateAdminRefreshToken(RefreshToken refreshToken)
        {
            context.ChangeTracker.Clear();
            var existingTokens = await context.RefreshTokens.Where(x => x.AdminId == refreshToken.AdminId).ToListAsync();

            if (existingTokens.Any())
                context.RefreshTokens.RemoveRange(existingTokens);

            context.RefreshTokens.Add(refreshToken);
            await context.SaveChangesAsync();
        }

        public async Task CreateAdmin(Admin admin)
        {
            context.ChangeTracker.Clear();
            context.Admins.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
