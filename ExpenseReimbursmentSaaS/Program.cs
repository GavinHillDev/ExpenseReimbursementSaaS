using ExpenseReimbursmentSaaS.Data;
using ExpenseReimbursmentSaaS.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseReimbursmentSaaS
{
    public class Program
    {
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
        }
        public static void Main(string[] args)
        {
            //JWT AUTH
            var builder = WebApplication.CreateBuilder(args);
            var key = builder.Configuration["Jwt:Key"];
            var issuer = builder.Configuration["Jwt:Issuer"];
            var audience = builder.Configuration["Jwt:Audience"];
            builder.Services.AddSingleton<JwtService>();

            builder.Services.AddAuthentication("Bearer")
             .AddJwtBearer("Bearer", options =>
             {
                 options.RequireHttpsMetadata = !builder.Environment.IsDevelopment(); ;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,

                     ValidateIssuerSigningKey = true,
                     ValidIssuer = builder.Configuration["Jwt:Issuer"],
                     ValidAudience = builder.Configuration["Jwt:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                 };

             });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("SaaSCors",
                    policy =>
                    {
                        policy
                        .SetIsOriginAllowed(origin =>
                        {
                            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                                return false;

                            return uri.Host.EndsWith(".ersaas.com");
                        })
                         .AllowAnyHeader()
                         .AllowAnyMethod();
                    }
                );
            });

            builder.Services.AddDbContext<ExpenseReimbursmentSaaSContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ExpenseReimbursmentSaaSContext") ?? throw new InvalidOperationException("Connection string 'ExpenseReimbursmentSaaSContext' not found.")));

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors("SaaSCors");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
