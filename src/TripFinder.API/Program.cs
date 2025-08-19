using Microsoft.EntityFrameworkCore;
using TripFinder.API.Extensions;
using TripFinder.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddCustomCors();
DotNetEnv.Env.Load();
// builder.Services.AddCustomHealthChecks(builder.Configuration);
// builder.Host.ConfigureLogging();
builder.Services.AddAuthorization();
// var db = builder.Configuration.GetConnectionString("DefaultConnection");
// Console.WriteLine(db, "This is the dab data");

builder.Services
    .AddAppDbContext(builder.Configuration)
    // .AddAppRepositories()
    // .AddAppServices()
    .AddAppSwagger();

var app = builder.Build();

var usedMemory = GC.GetTotalMemory(false);
Console.WriteLine(usedMemory);

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
context.Database.Migrate(); // Or context.Database.EnsureCreated();
// Seed.SeedData(scope.ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.UseHealthChecksUI();
}

// app.UseCustomHealthChecks();
app.UseCors("AllowClients");
// app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
