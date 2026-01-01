namespace UserService.src.DTOs
{
    public sealed class AuthResponseDTO
    {
        public bool IsSuccess { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public static AuthResponseDTO Success(Guid user_id, string username, string role) => new() { IsSuccess = true, UserId = user_id, Username = username, Role = role };

        public static AuthResponseDTO Failure() => new() { IsSuccess = false };
    }
}