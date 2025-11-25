using Briefly.Business.Interfaces;
using Briefly.Model.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Briefly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserDetails(Guid userId)
        {
            var userResponse = await _userService.GetUserDetailsAsync(userId);
            if (userResponse == null)
                return NotFound("User not found.");

            return Ok(userResponse);
        }

        [Authorize]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateUserAsync(userId, updateUserDto);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }


        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }
    }
}
