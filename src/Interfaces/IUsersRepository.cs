using UserService.src.DTOs;

namespace UserService.src.Interfaces
{
    public interface IUsersRepository
    {
        Task<IEnumerable<UsersListDTO>> GetAllAsync();
        Task<ResponseDTO> CreateAsync(UsersCreateDTO user);
        Task<ResponseDTO> UpdateAsync(UsersUpdateDTO user);
        Task<ResponseDTO> DeleteAsync(Guid id);

        // AuthService
        Task<AuthResponseDTO> AuthUserAsync(AuthRequestDTO auth);
        Task<AuthResponseDTO> GetUserByIdAsync(Guid id);
    }
}