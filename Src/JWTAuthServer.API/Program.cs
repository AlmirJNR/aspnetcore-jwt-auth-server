using System.Reflection;
using System.Text;
using API.Repositories;
using API.Services;
using Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region Database

builder.Services.AddDbContext<JWTContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
                      throw new ArgumentException()));

#endregion

#region Repositories

builder.Services.AddTransient<AppUserRepository>();

#endregion

#region Services

builder.Services.AddTransient<AppUserService>();

#endregion

#region Controllers

builder.Services.AddControllers();

#endregion

#region Authentication

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwtBearerOptions =>
{
    jwtBearerOptions.RequireHttpsMetadata = false;
    jwtBearerOptions.SaveToken = false;
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidateAudience = true,
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("JWT_KEY")
                ?? throw new Exception()))
    };
});

#endregion

#region CORS

builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy("AllowAll", configurePolicy =>
    {
        configurePolicy.AllowAnyHeader();
        configurePolicy.AllowAnyMethod();
        configurePolicy.AllowAnyOrigin();
    });
});

#endregion

#region Swagger/OpenAPI

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerGenOptions =>
{
    swaggerGenOptions.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "JWT Auth Server",
            Version = "v1",
            Contact = new OpenApiContact
            {
                Name = "Almir Junior", Email = "almirafjr.dev@gmail.com",
                Url = new Uri("https://www.linkedin.com/in/almir-j%C3%BAnior/")
            },
            Description = "A modularized JWT authentication service"
        });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    swaggerGenOptions.IncludeXmlComments(xmlPath);
});

#endregion

#region Build

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion