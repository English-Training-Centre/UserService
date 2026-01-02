using FluentValidation;
using FluentValidation.AspNetCore;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
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
                // Versioning API
                service.AddApiVersioning(opt =>
                {
                    opt.ReportApiVersions = true;
                    opt.AssumeDefaultVersionWhenUnspecified = true;
                    opt.DefaultApiVersion = new ApiVersion(1, 0);

                    opt.ApiVersionReader = ApiVersionReader.Combine(
                        new UrlSegmentApiVersionReader(),
                        new HeaderApiVersionReader("api-version")
                    );
                });
                service.AddVersionedApiExplorer(opt =>
                {
                    opt.GroupNameFormat = "'v'VVV";
                    opt.SubstituteApiVersionInUrl = true;
                }); 

                // Cache
                service.AddHttpCacheHeaders((op) => {
                        op.MaxAge = 60;
                        op.CacheLocation = CacheLocation.Private;
                    },
                    (validationOpt) => {
                        validationOpt.MustRevalidate = true;
                    }
                );                
                // end
                
                service.AddOpenApi();
                service.AddEndpointsApiExplorer();
                service.AddSwaggerConfiguration();
                service.AddJWTAuthentication(configuration, logger);
                service.AddHttpContextAccessor();
                service.AddValidatorsFromAssembly(typeof(Program).Assembly);

                // controllers config...
                service.AddControllers()
                .AddNewtonsoftJson()
                .Services.AddFluentValidationAutoValidation();
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

                // scoped
                service.AddScoped<IPostgresDbData, PostgresDB>();
                service.AddScoped<IRolesRepository, RolesRepository>();
                service.AddScoped<IUsersRepository, UsersRepository>();
                // end
                
                service.AddCors(op =>
                {
                    op.AddPolicy("CorsPolicy", c =>
                    {
                        c.WithOrigins("http://localhost:4200", "http://localhost:4201")
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