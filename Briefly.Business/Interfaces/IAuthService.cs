using Briefly.Model.DTOs;

namespace Briefly.Business.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterUserAsync(UserDto userDto);
        Task<UserResponseDto> LoginAsync(LoginDto loginDto);
    }
}
