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

// For additional default settigns in Aspire
builder.AddServiceDefaults();

// Add Azure Service Bus queue
builder.AddAzureServiceBusClient("servicebus");
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<Azure.Messaging.ServiceBus.ServiceBusClient>();
    return client.CreateSender("tasksQueue");
});

// Add services to the container.
builder.Services.AddControllers();

// Configure PostgreSQL with Entity Framework Core
builder.AddNpgsqlDbContext<ApplicationDbContext>(connectionName: "fair-share-db");

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

var app = builder.Build();

// Migrate the database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
