using Briefly.DataAccess.Interfaces;
using Briefly.Database;
using Briefly.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Briefly.DataAccess.Repositories
{
    public class UserRepository(BrieflyDbContext context) : IUserRepository
    {
        private readonly BrieflyDbContext _context = context;

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // Get a user by Id
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users
                                   .Include(u => u.Role)
                                   .FirstOrDefaultAsync(u => u.Id == userId);
            return user!;
        }

        // Get a user by Email (for login)
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                                   .Include(u => u.Role)
                                   .FirstOrDefaultAsync(u => u.Email == email);
            return user!;
        }

        // Get all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                                   .Include(u => u.Role)
                                   .ToListAsync();
        }

        // Update an existing user
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Delete a user by Id
        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
