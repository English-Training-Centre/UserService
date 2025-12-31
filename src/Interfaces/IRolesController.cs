using Microsoft.AspNetCore.Mvc;
using UserService.src.DTOs;

namespace UserService.src.Interfaces
{
    public interface IRolesController
    {
        Task<ActionResult<IEnumerable<RolesDTO>>> GetAllAsync();
        Task<IActionResult> CreateAsync([FromBody] RolesCreateDTO roles);
        Task<IActionResult> UpdateAsync([FromBody] RolesDTO roles);
        Task<IActionResult> DeleteAsync([FromRoute] Guid id);
    }
}