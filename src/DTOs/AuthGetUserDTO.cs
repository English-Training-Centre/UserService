namespace UserService.src.DTOs
{
    public sealed class AuthGetUserDTO
    {   
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}