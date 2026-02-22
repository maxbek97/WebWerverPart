using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebWerverPart.Models;
using WebWerverPart.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var dockerConnectionString = Environment.GetEnvironmentVariable("DB_HOST") != null
    ? $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
      $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
      $"Database={Environment.GetEnvironmentVariable("DB_DATABASE")};" +
      $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
      $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};"
    : null;

// 2) Если работаем локально → берем строку подключения из appsettings
var connectionString = dockerConnectionString
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<IvanvisionDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JWTService>();

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var secretKey = Environment.GetEnvironmentVariable("SECRETKEY")
                ?? builder.Configuration["AuthSettings:SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey!))
    };
});


var app = builder.Build();

// Авто-миграции
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IvanvisionDbContext>();

    var retries = 10;
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            Console.WriteLine("Migration successful");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine("Migration failed: " + ex.Message);
            Thread.Sleep(5000);
        }
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
