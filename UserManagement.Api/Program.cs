using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using UserManagement.Api.CustomerMiddlewares;
using UserManagement.Api.Data;
using UserManagement.Api.Mapper;
using UserManagement.Api.Repositorys;
using UserManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
    {
        var jwtSecuritySchema = new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Name = "JWT Authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Put only Bearer token on textbox below",
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };
        setup.AddSecurityDefinition(jwtSecuritySchema.Reference.Id, jwtSecuritySchema);
        setup.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { jwtSecuritySchema, Array.Empty<string>() }
        });
    }
    
    );
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "myCors",
        builder =>
        {
            builder.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true);
        });
});
builder.Services.AddDbContext<UsersDbContext>(option =>
{
    var connectionString = builder.Configuration.GetConnectionString("sqlserver");
    option.UseSqlServer(connectionString);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt key is not valid"))
            ),
        };
    });

builder.Services.AddScoped<IUsersRepo, UsersRepo>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<IHelperService, HelperService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new UserMapper(provider.GetRequiredService<UsersDbContext>()));
    cfg.AddProfile(new RoleMapper());
}).CreateMapper());

builder.Services.AddScoped<IDatabase>(cfg =>
{
    var multiplexer =
        ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("Redis:server") ?? throw new InvalidOperationException());
    return multiplexer.GetDatabase();
});
var app = builder.Build().SeedData();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("myCors");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CheckBlackListTokenMiddleware>();

app.MapControllers();

app.Run();
