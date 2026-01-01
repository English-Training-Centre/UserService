using Dapper;
using Npgsql;
using UserService.src.Configs;
using UserService.src.Constants;
using UserService.src.DTOs;
using UserService.src.Interfaces;

namespace UserService.src.Repositories
{
    public sealed class UsersRepository (IPostgresDbData db, IHttpContextAccessor httpContextAcc, ILogger<UsersRepository> logger) : IUsersRepository
    {
        private readonly IPostgresDbData _db = db;
        private readonly IHttpContextAccessor _httpContextAcc = httpContextAcc;
        private readonly ILogger<UsersRepository> _logger = logger;     
        
        public async Task<IEnumerable<UsersListDTO>> GetAllAsync()
        {
            const string sql = @"SELECT
                u.id,
                u.fullname,
                u.username,
                u.email,
                u.phone_number AS PhoneNumber,
                r.name AS Role,
                u.image_url AS ImageUrl,
                u.is_active AS IsActive,
                u.created_at AS CreatedAt,
                u.updated_at AS UpdatedAt
            FROM tbUsers u
            INNER JOIN tbRoles r ON r.id = u.role_id
            ORDER BY u.fullname ASC;";

            return await _db.QueryAsync<UsersListDTO>(sql);
        }

        public async Task<ResponseDTO> CreateAsync(UsersCreateDTO user)
        {
            try
            {
                return await _db.ExecuteInTransactionAsync(async (conn, tx) =>
                {
                    const string sqlExistName = @"SELECT 1 FROM tbUsers WHERE fullname = @Fullname;";
                    var existName = await conn.QueryFirstOrDefaultAsync<int>(sqlExistName, new { user.Fullname }, tx);
                    if (existName == 1) { return ResponseDTO.Failure(MessagesConstant.AlreadyExists); }

                    const string sqlExistRoleId = @"SELECT 1 FROM tbRoles WHERE id = @Id";
                    var existRoleId = await conn.QueryFirstOrDefaultAsync<int>(sqlExistRoleId, new {Id = user.RoleId}, tx);
                    if (existRoleId != 1) { return ResponseDTO.Failure(MessagesConstant.NotFound); }

                    var imageUrl = string.Empty;
                    if (existName == 0 && user.ImageUrl is not null && user.ImageUrl.Length > 0)
                    {
                        var fileName = await FileUploadConfig.UploadFile(user.ImageUrl);
                        var request = _httpContextAcc.HttpContext!.Request;
                        imageUrl = $"{request.Scheme}://{request.Host}/images/{fileName}";
                    }

                    const string sql = @"INSERT INTO tbUsers(fullname, username, email, phone_number, role_id, password_hash, image_url) VALUES(@Fullname, @Username, @Email, @PhoneNumber, @RoleId, @PasswordHash, @ImageUrl);";

                    var parameters = new
                    {
                        user.Fullname,
                        user.Username,
                        user.Email,
                        user.PhoneNumber,
                        user.RoleId,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                        ImageUrl = imageUrl
                    };

                    var result = await conn.ExecuteAsync(sql, parameters, tx);

                    return result == 0
                        ? ResponseDTO.Failure(MessagesConstant.OperationFailed)
                        : ResponseDTO.Success(MessagesConstant.Created);
                });
            }
            catch (PostgresException pgEx) when (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return ResponseDTO.Failure(MessagesConstant.AlreadyExists);
            }
            catch (PostgresException pgEx)
            {      
                _logger.LogError(pgEx, " - Unexpected PostgreSQL Error");                
                return ResponseDTO.Failure(MessagesConstant.DatabaseErrorGeneric);
            }
            catch (Exception ex)
            {  
                _logger.LogError(ex, " - Unexpected error during transaction operation.");                
                return ResponseDTO.Failure(MessagesConstant.UnexpectedError);
            }
        }

        public async Task<ResponseDTO> UpdateAsync(UsersUpdateDTO user)
        {
            try
            {
                return await _db.ExecuteInTransactionAsync(async (conn, tx) =>
                {
                    const string sqlExistId = @"SELECT 1 FROM tbUsers WHERE id = @Id;";
                    var existId = await conn.QueryFirstOrDefaultAsync<int>(sqlExistId, new {user.Id}, tx);
                    if (existId != 1) { return ResponseDTO.Failure(MessagesConstant.NotFound); }

                    const string sqlExistName = @"SELECT 1 FROM tbUsers WHERE fullname = @Fullname AND id != @Id;";
                    var existName = await conn.QueryFirstOrDefaultAsync<int>(sqlExistName, new {user.Id, user.Fullname}, tx);
                    if (existName == 1) { return ResponseDTO.Failure(MessagesConstant.AlreadyExists); }

                    const string sqlExistRoleId = @"SELECT 1 FROM tbRoles WHERE id = @Id";
                    var existRoleId = await conn.QueryFirstOrDefaultAsync<int>(sqlExistRoleId, new {Id = user.RoleId}, tx);
                    if (existRoleId != 1) { return ResponseDTO.Failure(MessagesConstant.NotFound); }

                    var updates = new List<string>();
                    var parameters = new DynamicParameters();

                    parameters.Add("@Id", user.Id);

                    if (!string.IsNullOrWhiteSpace(user.Fullname))
                    {
                        updates.Add("fullname = @Fullname");
                        parameters.Add("@Fullname", user.Fullname);
                    }

                    if (!string.IsNullOrWhiteSpace(user.Username))
                    {
                        updates.Add("username = @Username");
                        parameters.Add("@Username", user.Username);
                    }

                    updates.Add("email = @Email");
                    parameters.Add("@Email", user.Email);

                    updates.Add("phone_number = @PhoneNumber");
                    parameters.Add("@PhoneNumber", user.PhoneNumber);

                    updates.Add("role_id = @RoleId");
                    parameters.Add("@RoleId", user.RoleId);

                    const string sqlImageUrl = @"SELECT image_url FROM tbUsers WHERE id = @Id;";
                    var imagePath = await conn.QueryFirstOrDefaultAsync<string>(sqlImageUrl, new { user.Id }, tx);        
                    var imageUrl = string.Empty;

                    if (user.RemoveImage)
                    {
                    if (!string.IsNullOrEmpty(imagePath)) RemoveUserImage(imagePath);

                        updates.Add("image_url = @ImageUrl");
                        parameters.Add("@ImageUrl", imageUrl);
                    }
                    else if (user.ImageUrl is not null && user.ImageUrl.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(imagePath)) RemoveUserImage(imagePath);
                        
                        var fileName = await FileUploadConfig.UploadFile(user.ImageUrl);
                        var request = _httpContextAcc.HttpContext!.Request;
                        imageUrl = $"{request.Scheme}://{request.Host}/images/{fileName}";

                        updates.Add("image_url = @ImageUrl");
                        parameters.Add("@ImageUrl", imageUrl);
                    }

                    updates.Add("is_active = @IsActive");
                    parameters.Add("@IsActive", user.IsActive);

                    if (updates.Count == 1) { return ResponseDTO.Failure(MessagesConstant.NoChangesUpdate); }

                    var sql = $@"UPDATE tbUsers SET {string.Join(",", updates)} WHERE id = @Id;";
                    
                    var result = await conn.ExecuteAsync(sql, parameters, tx);

                    return result == 0
                        ? ResponseDTO.Failure(MessagesConstant.OperationFailed)
                        : ResponseDTO.Success(MessagesConstant.Updated);
                });
            }
            catch (PostgresException pgEx) when (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return ResponseDTO.Failure(MessagesConstant.AlreadyExists);
            }
            catch (PostgresException pgEx)
            {      
                _logger.LogError(pgEx, " - Unexpected PostgreSQL Error");                
                return ResponseDTO.Failure(MessagesConstant.DatabaseErrorGeneric);
            }
            catch (Exception ex)
            {  
                _logger.LogError(ex, " - Unexpected error during transaction operation.");                
                return ResponseDTO.Failure(MessagesConstant.UnexpectedError);
            }
        }

        public async Task<ResponseDTO> DeleteAsync(Guid id)
        {
            try
            {
                return await _db.ExecuteInTransactionAsync(async (conn, tx) =>
                {
                    const string sqlImageUrl = @"SELECT image_url FROM tbUsers WHERE id = @Id;";
                    var imagePath = await conn.QueryFirstOrDefaultAsync<string>(sqlImageUrl, new { Id = id }, tx);
                    if (!string.IsNullOrEmpty(imagePath)) RemoveUserImage(imagePath);

                    const string sql = @"DELETE FROM tbUsers WHERE id = @Id;";                    
                    var result = await conn.ExecuteAsync(sql, new { Id = id }, tx);
                    return result == 0
                    ? ResponseDTO.Failure(MessagesConstant.NotFound)
                    : ResponseDTO.Success(MessagesConstant.Deleted);
                });
            }
            catch (PostgresException pgEx)
            {      
                _logger.LogError(pgEx, " - Unexpected PostgreSQL Error");                
                return ResponseDTO.Failure(MessagesConstant.DatabaseErrorGeneric);
            }
            catch (Exception ex)
            {  
                _logger.LogError(ex, " - Unexpected error during transaction operation.");                
                return ResponseDTO.Failure(MessagesConstant.UnexpectedError);
            }
        }

        public async Task<AuthResponseDTO> AuthUserAsync(AuthRequestDTO auth)
        {
            const string sql = @"SELECT 
                u.id AS UserId,
                r.name AS Role,
                u.password_hash AS Password
                FROM tbUsers u
                JOIN tbRoles r ON r.id = u.role_id
            WHERE username = @Username
            AND is_active = TRUE
            LIMIT 1;";

            try
            {
                var getUser = await _db.QueryFirstOrDefaultAsync<AuthGetUserDTO>(sql, new { auth.Username });
                if (getUser is null || !BCrypt.Net.BCrypt.Verify(auth.Password, getUser.Password)) { return AuthResponseDTO.Failure(); }

                return AuthResponseDTO.Success(getUser.UserId, auth.Username, getUser.Role);
            }
            catch (PostgresException pgEx)
            {      
                _logger.LogError(pgEx, " - Unexpected PostgreSQL Error");                
                return AuthResponseDTO.Failure();
            }
            catch (Exception ex)
            {  
                _logger.LogError(ex, " - Unexpected error during transaction operation.");                
                return AuthResponseDTO.Failure();
            }
        }

        public async Task<AuthResponseDTO> GetUserByIdAsync(Guid id)
        {
            const string sql = @"SELECT
                u.id,
                u.username,
                r.name AS Role
                FROM tbUsers u
                JOIN tbRoles r ON r.id = u.role_id
            WHERE u.id = @Id
            AND u.is_active = TRUE
            LIMIT 1;";
            
            try
            {
                var getUser = await _db.QueryFirstOrDefaultAsync<AuthUserDTO>(sql, new { Id = id });
                if (getUser is null) { return AuthResponseDTO.Failure(); }

                return AuthResponseDTO.Success(getUser.Id, getUser.Username, getUser.Role);
            }
            catch (PostgresException pgEx)
            {      
                _logger.LogError(pgEx, " - Unexpected PostgreSQL Error");                
                return AuthResponseDTO.Failure();
            }
            catch (Exception ex)
            {  
                _logger.LogError(ex, " - Unexpected error during transaction operation.");                
                return AuthResponseDTO.Failure();
            }
        }

        private void RemoveUserImage(string imagePath)
        {
            try
            {
                var oldImageName = Path.GetFileName(new Uri(imagePath).LocalPath);
                if (!string.IsNullOrEmpty(oldImageName))
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    var oldImagePath = Path.Combine(uploadsFolder, oldImageName);

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image file.");
            }
        }
    }
}