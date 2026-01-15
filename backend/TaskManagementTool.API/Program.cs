using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Application.Services;
using TaskManagementTool.Infrastructure.Data;
using TaskManagementTool.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
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
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerDocumentUrlsPath = "/openapi/v1.json";
    });
}

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     await IdentitySeeder.SeedRolesAndAdmin(services);
// }

app.UseRouting();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();