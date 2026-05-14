using DailyPlanService.Context;
using DailyPlanService.MappingProfiles;
using FitTrackWithMLP.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;

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

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Enable cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: myAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:3000")
                                                        .AllowAnyHeader()
                                                        .AllowAnyMethod()
                                                        .AllowCredentials();

                                  });
            });

            builder.Services.AddFitTrackAuthentication(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(myAllowSpecificOrigins);

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
