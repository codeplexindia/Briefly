using AutoMapper;
using Briefly.Business.Interfaces;
using Briefly.DataAccess.Interfaces;
using Briefly.Model.DTOs;
using Briefly.Model.Entities;
using Microsoft.Extensions.Configuration;

namespace Briefly.Business.Services
{
    public class UserService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<UserResponseDto> GetUserDetailsAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return null;

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return new UserResponseDto { Success = false, Message = "User not found." };

            _mapper.Map(updateUserDto, user);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);
            return new UserResponseDto { Success = true, Message = "User updated successfully." };
        }

        public async Task<UserResponseDto> DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return new UserResponseDto { Success = false, Message = "User not found." };

            await _userRepository.DeleteUserAsync(user.Id);
            return new UserResponseDto { Success = true, Message = "User deleted successfully." };
        }
    }
}
