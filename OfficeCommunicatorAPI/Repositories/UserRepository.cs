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
        
        public async Task<User?> GetByIdentityAsync(UserIdentityPasswordDto userDto)
        {
            bool isEmail = _authHelper.IsEmail(userDto.Identity);
            if(isEmail) return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userDto.Identity);
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.UniqueName == userDto.Identity);
        }

        public Task<User?> GetByIdWithIncludeAsync(int id)
        {
            throw new NotImplementedException();
        }

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

            if(userDto.Name != null) user.Name = userDto.Name;
            if(userDto.Password != null)
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
    }
}