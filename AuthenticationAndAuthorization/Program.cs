
using AuthenticationAndAuthorization.Attributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace AuthenticationAndAuthorization
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //builder.Services.AddAuthorization(options =>
            //{
            //    List<string> policies = new List<string>()
            //    {
            //        "weather:read",
            //        "weather:write"
            //    };
            //    Parallel.ForEach(policies, policyItem =>
            //    {
            //        options.AddPolicy(policyItem, policy =>
            //        {
            //            policy.RequireClaim(policyItem);
            //        });
            //    });
            //});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.Use(async (context, next) =>
            {
                string? claimsString = context.Request.Headers.FirstOrDefault(x => x.Key == "claims").Value;

                if (!string.IsNullOrEmpty(claimsString))
                {
                    var claims = new List<Claim>();
                    foreach (var item in claimsString.Split(","))
                    {
                        claims.Add(new Claim(ClaimTypes.Name, item));
                    }
                    context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "MyAuthenticationScheme"));
                }
                await next();
            });

            app.Use(async (context, next) =>
            {
                var endpoint = context.GetEndpoint();
                var requiredClaim = endpoint.Metadata.GetMetadata<RequiredClaimAttribute>()?.Claim;

                if (!string.IsNullOrEmpty(requiredClaim) && !context.User.HasClaim(ClaimTypes.Name, requiredClaim))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden: Missing required claim.");
                    return;
                }
                await next();
            });
            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}
