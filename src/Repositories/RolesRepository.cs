using Dapper;
using Npgsql;
using UserService.src.Constants;
using UserService.src.DTOs;
using UserService.src.Interfaces;

namespace UserService.src.Repositories
{
    public sealed class RolesRepository(IPostgresDbData db, ILogger<RolesRepository> logger) : IRolesRepository
    {
        private readonly IPostgresDbData _db = db;

        private readonly ILogger<RolesRepository> _logger = logger;

        public async Task<IEnumerable<RolesDTO>> GetAllAsync()
        {
            const string sql = @"SELECT * FROM tbRoles ORDER BY Name ASC;";

            return await _db.QueryAsync<RolesDTO>(sql);
        }

        public async Task<ResponseDTO> CreateAsync(RolesCreateDTO roles)
        {

            try
            {
                return await _db.ExecuteInTransactionAsync(async (conn, tx) =>
                {
                    const string sqlExistName = @"SELECT 1 FROM tbRoles WHERE name = @Name;";
                    var existName = await conn.QueryFirstOrDefaultAsync<int>(sqlExistName, new {roles.Name}, tx);
                    if (existName == 1) { return ResponseDTO.Failure(MessagesConstant.AlreadyExists); }

                    const string sql = @"INSERT INTO tbRoles(name) VALUES(@Name);";
                    var result = await conn.ExecuteAsync(sql, new { roles.Name }, tx);

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

        public async Task<ResponseDTO> UpdateAsync(RolesDTO roles)
        {
            try
            {
                return await _db.ExecuteInTransactionAsync(async (conn, tx) =>
                {
                    const string sqlExistId = @"SELECT 1 FROM tbRoles WHERE id = @Id;";
                    var existId = await conn.QueryFirstOrDefaultAsync<int>(sqlExistId, new {roles.Id}, tx);
                    if (existId != 1) { return ResponseDTO.Failure(MessagesConstant.NotFound); }

                    const string sqlExistName = @"SELECT 1 FROM tbRoles WHERE name = @Name AND id != @Id;";
                    var existName = await conn.QueryFirstOrDefaultAsync<int>(sqlExistName, new {roles.Id, roles.Name}, tx);
                    if (existName == 1) { return ResponseDTO.Failure(MessagesConstant.AlreadyExists); }

                    const string sql = @"UPDATE tbRoles SET name = @Name WHERE id = @Id;";
                
                    var parameters = new
                    {
                        roles.Id,
                        roles.Name
                    };

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
                const string sql = @"DELETE FROM tbRoles WHERE id = @Id;";

                var result = await _db.ExecuteAsync(sql, new { Id = id });
                return result == 0
                    ? ResponseDTO.Failure(MessagesConstant.NotFound)
                    : ResponseDTO.Success(MessagesConstant.Deleted);
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
    }
}