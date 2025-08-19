using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DataCommunication.DataLibraries
{
    public class UserDataLibrary
    {
        public AppDbContext context { get; }

        public UserDataLibrary(AppDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task<bool> GetUserByNumber(string number)
        {
           return await context.Users.AnyAsync(u => u.MobileNumber == number && !u.IsDeleted);
        }

        public async Task<User> GetUser(Guid Id)
        {
            return await context.Users.Include(x => x.UserType)
                .Include(x => x.IndividualProfile).Include(x => x.CompanyProfile).Include(x => x.DesignerProfile).AsNoTracking().AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == Id && !x.IsDeleted);
        }

        public async Task<User> GetUserOnly(Guid Id)
        {
            return await context.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<string> GetUserByCodeAndNumber(string? code, string? number)
        {
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(number))
                return await context.Users.Where(x => x.CountryCode == code && x.MobileNumber == number).Select(x => x.Id.ToString()).FirstOrDefaultAsync();
            else
                return null;
        }

        public async Task<int> GetTotalUsers()
        {
            return await context.Users.CountAsync();
        }

        public async Task<int> GetTotalIndividualUsers()
        {
            return await context.Users.CountAsync(u => u.UserTypeId == 1);
        }

        public async Task<int> GetCompanyUsers()
        {
            return await context.Users.CountAsync(u => u.UserTypeId == 2);
        }

        public async Task<int> GetDesignerUsers()
        {
            return await context.Users.CountAsync(u => u.UserTypeId == 3);
        }

        public async Task<int> GetBlockedUsers()
        {
            return await context.Users.CountAsync(u => u.IsDeleted && u.Status == "Blocked");
        }

        public async Task<List<User>> GetUsersByName(string name)
        {
            var loweredName = name.ToLower();

            return await context.Users.Include(x => x.UserType)
                .Include(u => u.IndividualProfile)
                .Include(u => u.CompanyProfile)
                .Include(u => u.DesignerProfile)
                .Where(u => !string.IsNullOrEmpty(u.Name) && u.Name.ToLower().Contains(name) && !u.IsDeleted
                ).AsNoTracking().AsSplitQuery()
                .ToListAsync();
        }


        public async Task<User> GetUserById(Guid Id)
        {
            return await context.Users
                .Include(u => u.UserType)
                .Include(u => u.IndividualProfile)
                .Include(u => u.CompanyProfile)
                .Include(u => u.DesignerProfile).AsNoTracking().AsSplitQuery()
                .FirstOrDefaultAsync(u => u.Id == Id && !u.IsDeleted);
        }

        public async Task<User> CreateUser(User user)
        {
            context.ChangeTracker.Clear();
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<IndividualUser> CreateIndividual(IndividualUser individualUser)
        {
            context.ChangeTracker.Clear();
            context.IndividualUsers.Add(individualUser);
            await context.SaveChangesAsync();
            return individualUser;
        }

        public async Task<CompanyUser> CreateCompanyUser(CompanyUser companyUser)
        {
            context.ChangeTracker.Clear();
            context.CompanyUsers.Add(companyUser);
            await context.SaveChangesAsync();
            return companyUser;
        }

        public async Task<DesignerUser> CreateDesigner(DesignerUser designerUser)
        {
            context.ChangeTracker.Clear();
            context.DesignerUsers.Add(designerUser);
            await context.SaveChangesAsync();
            return designerUser;
        }

        public async Task UpdateUser(User user)
        {
            context.ChangeTracker.Clear();
            context.Entry(user).State = EntityState.Modified;

            if (user.IndividualProfile != null)
                context.Entry(user.IndividualProfile).State = EntityState.Modified;

            if (user.CompanyProfile != null)
                context.Entry(user.CompanyProfile).State = EntityState.Modified;

            if (user.DesignerProfile != null)
                context.Entry(user.DesignerProfile).State = EntityState.Modified;

            await context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await context.Users
                .Include(u => u.UserType)
                .Include(u => u.IndividualProfile)
                .Include(u => u.CompanyProfile)
                .Include(u => u.DesignerProfile).Where(x => !x.IsDeleted).AsSplitQuery().AsNoTracking().ToListAsync();
        }

        public async Task<List<User>> GetAllUsersAsync(int? userTypeId, bool? isDeleted)
        {
            var query = context.Users
                .Include(u => u.UserType)
                .Include(u => u.IndividualProfile)
                .Include(u => u.CompanyProfile)
                .Include(u => u.DesignerProfile).AsSplitQuery().AsNoTracking()
                .AsQueryable();

            if (userTypeId.HasValue)
                query = query.Where(u => u.UserTypeId == userTypeId.Value);

            if (isDeleted.HasValue)
                query = query.Where(u => u.IsDeleted == isDeleted.Value);

            return await query.ToListAsync();
        }
    }
}
