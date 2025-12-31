namespace UserService.src.DTOs
{
    public class UsersCreateDTO
    {
        public string Fullname { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }        
        public Guid RoleId { get; set; }
        public string Password { get; set; } = string.Empty;        
        public IFormFile? ImageUrl { get; set; }
    }
}