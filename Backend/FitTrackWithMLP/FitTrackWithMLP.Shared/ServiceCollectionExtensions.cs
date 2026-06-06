using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FitTrackWithMLP.Shared
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFitTrackAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            
            return services;
        }
    }
}
