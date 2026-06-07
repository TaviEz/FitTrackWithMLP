using DailyPlanService.Context;
using DailyPlanService.MappingProfiles;
using DailyPlanService.Services.DailyPlan;
using DailyPlanService.Services.MealOptimzer;
using FitTrackWithMLP.Shared;
using FitTrackWithMLP.Shared.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using System.Text.Json.Serialization;
using DailyPlanServiceImpl = DailyPlanService.Services.DailyPlan.DailyPlanService;

namespace DailyPlanService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
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
                cfg.AddProfile<DailyPlanProfile>();    
                cfg.LicenseKey = mapperConfiguration["LicenseKey"];
            });

            builder.Services.AddTransient<GlobalExceptionMiddleware>();

            builder.Services.AddScoped<IDailyPlanService, DailyPlanServiceImpl>();
            builder.Services.AddHttpClient<IMealOptimizerClient, MealOptimizerClient>((sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(config["Optimizer:Url"]!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

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

            builder.Services.AddFitTrackAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();

            app.UseCors(myAllowSpecificOrigins);

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
