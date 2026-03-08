using ExpenseReimbursmentSaaS.Data;
using ExpenseReimbursmentSaaS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ExpenseReimbursmentSaaS
{
    public class Program
    {

         public static void Main(string[] args)
        {
              
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
            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                 
                app.MapOpenApi();
            }
            //TEST Admin
            //using (var scope = app.Services.CreateScope()) {
            //    var context = scope.ServiceProvider.GetRequiredService<ExpenseReimbursmentSaaSContext>();
            //    context.Database.EnsureCreated();
            //    var passwordHasher = new PasswordHasher<Employee>();
            //    if (app.Environment.IsDevelopment())
            //    {
            //        context.Employee.RemoveRange(context.Employee);
            //        context.SaveChanges();
            //        Console.WriteLine("Removed Admin");
            //    }
            //    //_passwordHasher = new PasswordHasher<Employee>();
            //    if (!context.Employee.Any(a => a.Role == Roles.Admin))
            //    {
            //        //TEST Admin
            //        var admin = new Employee
            //        {
            //            Role = Roles.Admin,
            //            Email = "testadmin@test.com",
            //            Name = "admin",
            //            PasswordHash = passwordHasher.HashPassword(null, "123")
            //        };
            //        context.Employee.Add(admin);
            //        context.SaveChanges();
            //        Console.WriteLine("Added Admin");
            //    }
            //}



            app.UseCors("SaaSCors");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
