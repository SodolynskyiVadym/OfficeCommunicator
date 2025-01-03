using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;

namespace OfficeCommunicatorAPI.Repositories
{
    public class UserRepository : IRepository<User, UserDto, UserUpdateDto>
    {
        private readonly OfficeDbContext _dbContext;
        private readonly AuthHelper _authHelper;
        private readonly IMapper _mapper;

        public UserRepository(OfficeDbContext dbContext, IMapper mapper, AuthHelper authHelper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authHelper = authHelper;
        }
        
        public async Task<List<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }


        public async Task<User?> GetByIdWithIncludeAsync(int id)
        {
            return await _dbContext.Users
                .Include(u => u.Groups)
                .Include(u => u.Contacts)
                .ThenInclude(c => c.AssociatedUser)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(UserEmailPasswordDto userDto)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
        }

        //public async Task<UserPresentDto?> GetUserPresentByIdWithIncludeAsync(int id)
        //{
        //    User? user = await _dbContext.Users
        //        .Include(u => u.Groups)
        //        .Include(u => u.Contacts)
        //        .ThenInclude(c => c.AssociatedUser)
        //        .FirstOrDefaultAsync(u => u.Id == id);
            
        //    if (user == null) return null;
        //    return _mapper.Map<UserPresentDto>(user);
        //}

        public async Task<User> AddAsync(UserDto userDto)
        {
            byte[][] passwordData = _authHelper.EncryptUserPassword(userDto.Password);
            var user = _mapper.Map<(UserDto, byte[][]), User>((userDto, passwordData));
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateAsync(UserUpdateDto userDto)
        {
            User? user = await _dbContext.Users.FindAsync(userDto.Id);
            if (user == null) return false;

            user.Name = userDto.Name ?? user.Name;
            user.AzureToken = userDto.AzureToken ?? user.AzureToken;
            user.ZoomUrl = userDto.ZoomUrl ?? user.ZoomUrl;

            if (userDto.Password != null)
            {
                byte[][] passwordData = _authHelper.EncryptUserPassword(userDto.Password);
                user.PasswordHash = passwordData[0];
                user.PasswordSalt = passwordData[1];
            }
            int result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            User? user = await _dbContext.Users.FindAsync(id);
            if (user == null) return false;
            
            _dbContext.Users.Remove(user);
            int result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }


        public async Task<bool> IsUserGroup(int userId, int groupId)
        {
            User? user =  await _dbContext.Users
                .Include(u => u.Groups)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return false;

            return user.Groups.Any(g => g.Id == groupId);
        }
    }
}