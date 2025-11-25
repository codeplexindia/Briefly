using Briefly.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Briefly.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserDetailsAsync(Guid userId);
        Task<UserResponseDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
        Task<UserResponseDto> DeleteUserAsync(Guid userId);
    }
}
