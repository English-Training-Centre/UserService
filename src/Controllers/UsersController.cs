using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.src.Constants;
using UserService.src.DTOs;
using UserService.src.Interfaces;

namespace UserService.src.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUsersRepository usersRepository) : ControllerBase, IUsersController
    {
        private readonly IUsersRepository _usersRep = usersRepository;

        [HttpGet("v1/get-all")]
        public async Task<ActionResult<IEnumerable<UsersListDTO>>> GetAllAsync()
        {
            var result = await _usersRep.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("v1/create")]
        public async Task<IActionResult> CreateAsync([FromForm] UsersCreateDTO user)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _usersRep.CreateAsync(user);

            if (!result.IsSuccess)
                return result.Message == MessagesConstant.AlreadyExists ? Conflict(result) : BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("v1/update")]
        public async Task<IActionResult> UpdateAsync([FromForm] UsersUpdateDTO user)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _usersRep.UpdateAsync(user);

            if (!result.IsSuccess)
                return result.Message == MessagesConstant.NotFound ? NotFound(result) : BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("v1/delete/{id:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _usersRep.DeleteAsync(id);

            if (!result.IsSuccess)
                return result.Message == MessagesConstant.NotFound ? NotFound(result) : BadRequest(result);

            return Ok(result);
        }

        // AuthService
        [AllowAnonymous]
        [HttpPost("v1/auth")]
        public async Task<ActionResult<AuthResponseDTO>> AuthUserAsync([FromBody] AuthUsersDTO user)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _usersRep.AuthUserAsync(user);

            return Ok(result);
        }
    }
}