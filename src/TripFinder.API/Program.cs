using Microsoft.EntityFrameworkCore;
using TripFinder.API.Extensions;
using TripFinder.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
DotNetEnv.Env.Load();
builder.Services.AddAuthorization();

builder.Services
    .AddAppDbContext(builder.Configuration)
    .AddAppSwagger();

var app = builder.Build();

var usedMemory = GC.GetTotalMemory(false);
Console.WriteLine(usedMemory);

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
context.Database.Migrate(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
