using ExpenseReimbursmentSaaS.Data;
using ExpenseReimbursmentSaaS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseReimbursmentSaaS
{
    public class Program
    {
        //public static string HashPassword(string password)
        //{
        //    byte[] salt = new byte[128 / 8];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(salt);
        //    }

        //    return Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //        password: password,
        //        salt: salt,
        //        prf: KeyDerivationPrf.HMACSHA256,
        //        iterationCount: 10000,
        //        numBytesRequested: 256 / 8
        //    ));
        //}

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
            builder.Services.AddControllers();
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
            
            using (var scope = app.Services.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<ExpenseReimbursmentSaaSContext>();
                context.Database.EnsureCreated();
                var passwordHasher = new PasswordHasher<Employee>();
                if (app.Environment.IsDevelopment())
                {
                    context.Employee.RemoveRange(context.Employee);
                    context.SaveChanges();
                    Console.WriteLine("Removed Admin");
                }
                //_passwordHasher = new PasswordHasher<Employee>();
                if (!context.Employee.Any(a => a.Role == Roles.Admin))
                {

                    var admin = new Employee
                    {
                        Role = Roles.Admin,
                        Email = "testadmin@test.com",
                        Name = "admin",
                        PasswordHash = passwordHasher.HashPassword(null, "123")
                    };
                    //var employee = new Employee
                    //{
                    //    Role = Roles.Admin,
                    //    Email = "employee4@test.com",
                    //    Name = "employee",
                    //    PasswordHash = passwordHasher.HashPassword(null, "111")
                    //};
                    context.Employee.Add(admin);
                   // context.Employee.Add(employee);
                    context.SaveChanges();
                    Console.WriteLine("Added Admin");
                }
            }


            //app.UseHttpsRedirection();

            app.UseCors("SaaSCors");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
