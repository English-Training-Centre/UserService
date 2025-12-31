using UserService.src.DTOs;

namespace UserService.src.Interfaces
{
    public interface IRolesRepository
    {
        Task<IEnumerable<RolesDTO>> GetAllAsync();
        Task<ResponseDTO> CreateAsync(RolesCreateDTO roles);
        Task<ResponseDTO> UpdateAsync(RolesDTO roles);
        Task<ResponseDTO> DeleteAsync(Guid id);
    }
}