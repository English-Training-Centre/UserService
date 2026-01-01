namespace UserService.src.DTOs
{
    public sealed class AuthGetUserDTO
    {   
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}