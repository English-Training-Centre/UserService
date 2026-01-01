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
    public class RolesController(IRolesRepository rolesRepository) : ControllerBase, IRolesController
    {
        private readonly IRolesRepository _rolesRep = rolesRepository;

        [HttpGet("v1/get-all")]
        public async Task<ActionResult<IEnumerable<RolesDTO>>> GetAllAsync()
        {
            var result = await _rolesRep.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("v1/create")]
        public async Task<IActionResult> CreateAsync([FromBody] RolesCreateDTO roles)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _rolesRep.CreateAsync(roles);

            if (!result.IsSuccess)
                return result.Message == MessagesConstant.AlreadyExists ? Conflict(result) : BadRequest(result);

            return Ok(result);
        }

        [HttpPut("v1/update")]
        public async Task<IActionResult> UpdateAsync([FromBody] RolesDTO roles)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _rolesRep.UpdateAsync(roles);

            if (!result.IsSuccess)
                return result.Message == MessagesConstant.NotFound ? NotFound(result) : BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("v1/delete/{id:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ResponseDTO.Failure(MessagesConstant.InvalidData));

            var result = await _rolesRep.DeleteAsync(id);

            if (!result.IsSuccess)
                return result.Message == MessagesConstant.NotFound ? NotFound(result) : BadRequest(result);

            return Ok(result);
        }
    }
}