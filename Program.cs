/*
* Copyright (c) 2025, Ramadan Ibraimo Abdula Ismael - Mozambique, Maputo - All rights reserved
*/

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using UserService.src.Configs;

var builder = WebApplication.CreateBuilder(args);

ServiceManagerConfig.Configure(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwaggerConfiguration();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseWebSockets();

app.UseRouting();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
if (!Directory.Exists(path)) Directory.CreateDirectory(path);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = "/" + "images"
});

app.MapControllers();

app.Run();