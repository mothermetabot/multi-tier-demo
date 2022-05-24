using Microsoft.AspNetCore.Builder;
using Application2.Controllers;
using Application2.Services;
using Microsoft.Extensions.FileProviders;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication
    .CreateBuilder(args);

builder.WebHost.UseKestrel();

// Add services to the container.

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IMessageValidator, DefaultMessageValidator>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors();

var app = builder.Build();

app.Logger.LogTrace(builder.Environment.WebRootPath);

app.UseCors(builder =>
   builder.WithOrigins("http://localhost:44475", "http://localhost:5000")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials());

app.UseStatusCodePages(Text.Plain, "Status Code Page: {0}");

app.UseStaticFiles();
app.UseRouting();

app.MapHub<MainHub>("/main");
app.MapFallbackToFile("index.html", new StaticFileOptions() {
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(builder.Environment.WebRootPath)
});

app.Run();
