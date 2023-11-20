using System.Text;
using BiddingSystem.Context;
using BiddingSystem.Services;
using BiddingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Hangfire;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<BiddingSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("bidding-system-context") ??
                         throw new InvalidOperationException("Connection string 'bidding-system-context' not found.")));

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("bidding-system-context") ??
                               throw new InvalidOperationException(
                                   "Connection string 'bidding-system-context' not found.")));
builder.Services.AddHangfireServer();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuctionService, AuctionService>();
builder.Services.AddScoped<IBiddingService, BiddingService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
//     {
//         In = ParameterLocation.Header,
//         Name = "Authorization",
//         Type = SecuritySchemeType.ApiKey
//     });
//     
//     options.OperationFilter<SecurityRequirementsOperationFilter>();
// });

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = "BiddingSystemApi",
//         ValidAudience = "http://localhost:5068",
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
//             builder.Configuration.GetSection("JwtSettings:Token").Value!))
//         
//     };
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();