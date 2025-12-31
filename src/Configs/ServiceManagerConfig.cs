using FluentValidation;
using FluentValidation.AspNetCore;
using Npgsql;
using UserService.src.DB;
using UserService.src.Interfaces;
using UserService.src.Repositories;

namespace UserService.src.Configs
{
    public static class ServiceManagerConfig
    {
        public static void Configure(IServiceCollection service, IConfiguration configuration)
        {
            var logger = LoggerFactory.Create(s => s.AddConsole()).CreateLogger<Program>();

            try
            {
                service.AddOpenApi();
                service.AddEndpointsApiExplorer();
                service.AddSwaggerConfiguration();
                service.AddJWTAuthentication(configuration, logger);
                service.AddHttpContextAccessor();

                // FluentValidation
                service.AddValidatorsFromAssembly(typeof(Program).Assembly);
                service.AddControllers().Services.AddFluentValidationAutoValidation();
                // end

                service.AddSingleton<NpgsqlDataSource>(sp =>
                {
                    var builder = new NpgsqlDataSourceBuilder(
                        PostgresDB.BuildConnectionStringFromEnvironment());

                    builder.UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>());

                    return builder.Build();
                });

                service.AddHealthChecks()
                    .AddNpgSql(sp => sp.GetRequiredService<NpgsqlDataSource>());

                service.AddScoped<IPostgresDbData, PostgresDB>();
                service.AddScoped<IRolesRepository, RolesRepository>();
                service.AddScoped<IUsersRepository, UsersRepository>();

                string? audience = configuration["JWTSettings:validAudience"];
                if (string.IsNullOrWhiteSpace(audience)) throw new InvalidOperationException("JWTSettings:validAudience is missing in configuration.");
                
                service.AddCors(op =>
                {
                    op.AddPolicy("CorsPolicy",
                    c =>
                    {
                        c.WithOrigins(audience)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
                });
            }
            catch (Exception ex)
            {
                logger.LogError("An error occurred while configuring services: {Message}", ex.Message );
            }
        }
    }
}