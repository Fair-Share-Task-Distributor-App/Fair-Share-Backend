using System.Text;
using Fair_Share.Api;
using Fair_Share.Api.Data;
using Fair_Share.Api.Mappers;
using Fair_Share.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// For Swagger UI
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<AppExceptionHandler>();

// Add services to the container.
builder.Services.AddControllers();

// Configure PostgreSQL with Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<TaskPreferenceService>();
builder.Services.AddScoped<AccountService>();

// Register Mappers
builder.Services.AddScoped<AccountMapper>();
builder.Services.AddScoped<TaskMapper>();
builder.Services.AddScoped<TeamMapper>();
builder.Services.AddScoped<TaskPreferenceMapper>();

// Configure JWT Authentication
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)
            )
        };
    });

builder.Services.AddAuthorization();

// Add CORS if needed for frontend
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(
//        "AllowFrontend",
//        policy =>
//        {
//            policy
//                .WithOrigins(builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? [])
//                .AllowAnyMethod()
//                .AllowAnyHeader()
//                .AllowCredentials();
//        }
//    );
//});

var app = builder.Build();

// Add exception handler middleware
app.UseExceptionHandler(options => { });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

app.UseRouting();

//app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
