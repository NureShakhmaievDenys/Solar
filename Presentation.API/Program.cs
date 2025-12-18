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

namespace Presentation.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Отримуємо ConnectionString
            // DigitalOcean сам підставить сюди правильний рядок з Environment Variables
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // 2. Налаштування DbContext
            // ВАЖЛИВО: Змінили UseSqlServer на UseNpgsql (для PostgreSQL)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            // 3. Реєстрація інтерфейсу та сервісів
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
                    Description = "Введіть 'Bearer' [пробіл] і ваш токен.\n\nПриклад: 'Bearer 12345abcdef'",
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

            // 5. Налаштування JWT Автентифікації
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
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)
                    )
                };
            });

            // 6. Додаємо Авторизацію
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // === ВАЖЛИВО: АВТОМАТИЧНА МІГРАЦІЯ БАЗИ ===
            // Це створює таблиці в порожній базі DigitalOcean при запуску
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    context.Database.Migrate(); // Застосовує всі міграції
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Сталася помилка під час міграції бази даних.");
                }
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}