using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FitTrackWithMLP.Shared
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFitTrackAntiforgeryCookie(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            bool isDev = env.IsDevelopment();

            app.Use(async (context, next) =>
            {
                // issue a new antiforgery token cookie for GET requests if one doesn't exist
                if ((HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method)) &&
                    !context.Request.Cookies.ContainsKey("XSRF-TOKEN"))
                {
                    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
                    var tokens = antiforgery.GetAndStoreTokens(context);

                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = !isDev,
                        SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None
                    });
                }

                await next(context);
            });

            return app;
        }

        public static IApplicationBuilder UseFitTrackAntiforgeryValidation(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var isUnsafe =
                    HttpMethods.IsPost(context.Request.Method) ||
                    HttpMethods.IsPut(context.Request.Method) ||
                    HttpMethods.IsPatch(context.Request.Method) ||
                    HttpMethods.IsDelete(context.Request.Method);

                if (!isUnsafe)
                {
                    await next();
                    return;
                }

                // Bypasses Identity API endpoints (login, register, refresh) safely
                if (context.Request.Path.StartsWithSegments("/api/user/login") ||
                    context.Request.Path.StartsWithSegments("/api/user/register"))
                {
                    await next();
                    return;
                }

                var endpoint = context.GetEndpoint();
                var allowAnonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null;

                if (allowAnonymous)
                {
                    await next();
                    return;
                }

                var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();

                try
                {
                    await antiforgery.ValidateRequestAsync(context);
                }
                catch (AntiforgeryValidationException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid CSRF token." });
                    return;
                }

                await next();
            });

            return app;
        }
    }
}
