using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.src.Constants;
using UserService.src.DTOs;
using UserService.src.Interfaces;

namespace UserService.src.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController(IUsersRepository usersRepository) : ControllerBase, IUsersController
    {
        private readonly IUsersRepository _usersRep = usersRepository;

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<UsersListDTO>>> GetAllAsync()
        {
            var result = await _usersRep.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromForm] UsersCreateDTO user)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _usersRep.CreateAsync(user);

            if (!result.IsSuccess)
                return result.Message == MessagesConstant.AlreadyExists ? Conflict(result) : BadRequest(result);

            return Ok(result);
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateAsync([FromForm] UsersUpdateDTO user)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _usersRep.UpdateAsync(user);

            if (!result.IsSuccess)
                return result.Message == MessagesConstant.NotFound ? NotFound(result) : BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("delete/{id:guid}")]
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
        [HttpPost("auth")]
        public async Task<ActionResult<AuthResponseDTO>> AuthUserAsync([FromBody] AuthRequestDTO auth)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _usersRep.AuthUserAsync(auth);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("get-user/{id:guid}")]
        public async Task<ActionResult<AuthResponseDTO>> GetUserByIdAsync([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _usersRep.GetUserByIdAsync(id);

            return Ok(result);
        }
    }
}