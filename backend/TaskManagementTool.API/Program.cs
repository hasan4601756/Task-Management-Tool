using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Application.Services;
using TaskManagementTool.Infrasrtucure.Identity;
using TaskManagementTool.Infrastructure.Data;
using TaskManagementTool.Infrastructure.Identity;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services
    .AddDbContext<AppDbContext>(
        options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
    
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();