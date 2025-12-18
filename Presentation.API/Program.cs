using Application.Interfaces;
using Application.Services;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Npgsql; 

namespace Presentation.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Отримуємо ConnectionString
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // === ИСПРАВЛЕНИЕ ДЛЯ DIGITALOCEAN ===
            // Если строка пришла в формате URL (как дает DO), переделываем её в нормальный вид
            if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("://"))
            {
                connectionString = ConvertUrlConnectionString(connectionString);
            }
            // =====================================

            // 2. Налаштування DbContext (PostgreSQL)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            // 3. Реєстрація сервісів
            builder.Services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<AppDbContext>());

            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<SiteService>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<StatisticsService>();
            builder.Services.AddScoped<TelemetryService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Введіть 'Bearer' [пробіл] і ваш токен",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // 5. JWT
            // ВАЖНО: Убедитесь, что добавили переменные JwtSettings__Key в DigitalOcean!
            var jwtKey = builder.Configuration["JwtSettings:Key"];
            var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
            var jwtAudience = builder.Configuration["JwtSettings:Audience"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    // Проверка на случай, если ключ не задан (чтобы не упало с ошибкой)
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey ?? "TemporaryKeyForMigrationAndDevBuild123!")
                    )
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // === АВТО-МИГРАЦИЯ ===
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Помилка міграції бази даних.");
                }
            }

            // Включаем Swagger всегда (и в Production)
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        // === ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ КОНВЕРТАЦИИ URL ===
        private static string ConvertUrlConnectionString(string url)
        {
            if (!url.Contains("//")) return url;

            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = uri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require, // DigitalOcean требует SSL
                TrustServerCertificate = true // Доверяем сертификату DO
            };

            return builder.ToString();
        }
    }
}