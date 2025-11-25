using AutoMapper;
using Briefly.Business.Interfaces;
using Briefly.DataAccess.Interfaces;
using Briefly.Model.DTOs;
using Briefly.Model.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Briefly.Business.Services
{
    public class AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _configuration = configuration;

        public async Task<UserResponseDto> RegisterUserAsync(UserDto userDto)
        {
            if (await _userRepository.UserExistsAsync(userDto.Email))
                return new UserResponseDto { Success = false, Message = "User already exists!" };

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password) // Hash password
            };

            await _userRepository.AddUserAsync(user);
            return new UserResponseDto { Success = true, Message = "User registered successfully!" };
        }

        public async Task<UserResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return new UserResponseDto { Success = false, Message = "Invalid email or password." };
            }

            var token = GenerateJwtToken(user);

            return new UserResponseDto { Success = true, Message = "Login successful.", Token = token };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role!.RoleName) // Assuming Role is a navigational property
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
