using Microsoft.AspNetCore.Mvc;
using UserService.src.DTOs;

namespace UserService.src.Interfaces
{
    public interface IUsersController
    {
        Task<ActionResult<IEnumerable<UsersListDTO>>> GetAllAsync();
        Task<IActionResult> CreateAsync([FromForm] UsersCreateDTO user);
        Task<IActionResult> UpdateAsync([FromForm] UsersUpdateDTO user);
        Task<IActionResult> DeleteAsync([FromRoute] Guid id);

        // AuthService
        Task<ActionResult<AuthResponseDTO>> AuthUserAsync([FromBody] AuthRequestDTO auth);
        Task<ActionResult<AuthResponseDTO>> GetUserByIdAsync([FromRoute] Guid id);
    }
}