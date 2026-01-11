using Microsoft.AspNetCore.Identity;
using TaskManagementTool.Domain.Enums;
using TaskManagementTool.Infrasrtucure.Identity;
using TaskManagementTool.Infrastructure.Data;
using TaskManagementTool.Infrastructure.Identity;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
    
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();