using FitTrackWithMLP.Shared;
using FitTrackWithMLP.Shared.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using UserManagementService.Context;
using UserManagementService.MappingProfiles;
using UserManagementService.Models;
using UserManagementService.Services.Authentication;
using UserManagementService.Services.UserDetails;

namespace UserManagementService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var mapperConfiguration = builder.Configuration.GetSection("AutoMapper");

            // read serilog config from appsettings.json
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? new[] { "http://localhost:3000" };

            builder.Host.UseSerilog();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = []
                });
            });

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<UserDetailsProfile>();
                cfg.LicenseKey = mapperConfiguration["LicenseKey"];
            });

            builder.Services.AddTransient<GlobalExceptionMiddleware>();

            builder.Services.AddScoped<IIdentityService, IdentityService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IUserDetailsService, UserDetailsService>();

            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(connectionString, sqlOptions =>
                    sqlOptions.EnableRetryOnFailure()));

            // Add Identity
            builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddFitTrackAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();

            // Enable cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: myAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins(allowedOrigins)
                                                        .AllowAnyHeader()
                                                        .AllowAnyMethod()
                                                        .AllowCredentials();
                                          
                                  });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseCors(myAllowSpecificOrigins);

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapIdentityApi<ApplicationUser>();

            app.MapControllers();

            app.Run();
        }
    }
}
