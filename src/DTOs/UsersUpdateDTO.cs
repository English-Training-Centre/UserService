namespace UserService.src.DTOs
{
    public class UsersUpdateDTO
    {        
        public Guid Id { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }        
        public Guid RoleId { get; set; }        
        public IFormFile? ImageUrl { get; set; }
        public bool RemoveImage { get; set; }
        public bool IsActive { get; set; }
    }
}