using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Mappers;
using API.Models;
using API.UnitOfWorks;
using API.Hubs;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddDbContext<BlueHorizonDbContext>(options => options
                .UseLazyLoadingProxies()
                .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                // options.Password.RequireUppercase = false; // ⚠️ Removed duplicate
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 3;
            })
                .AddEntityFrameworkStores<BlueHorizonDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddOpenApi();
            builder.Services.AddSignalR();

            // ✅ CHANGE START: Modified AddAuthorization to define a policy
            builder.Services.AddAuthorization(options =>
            {
                // Define an Authorization Policy named "myschema".
                // This policy requires that the user is authenticated (has a valid token).
                options.AddPolicy("myschema", policy =>
                {
                    policy.RequireAuthenticatedUser(); // This policy ensures the user is authenticated.
                    // If you wanted to specify which authentication scheme to use for this policy, you could add:
                    // policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme); 
                    // However, since "myschema" is your DefaultAuthenticateScheme, it will implicitly use it.
                });
            });
            // ✅ CHANGE END: Modified AddAuthorization to define a policy

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingConfig>());

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddAuthentication(options => {
                options.DefaultScheme = "myschema";
                options.DefaultAuthenticateScheme = "myschema";
                options.DefaultChallengeScheme = "myschema";
            })
            .AddJwtBearer("myschema", option =>
            {
                var key = "this is secrete key for admin role base";
                var secreteKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = secreteKey,
                    NameClaimType = ClaimTypes.NameIdentifier
                };

                option.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/chathub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200", "https://localhost:4200");
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                        builder.AllowCredentials();
                    });
            });


            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // ✅ No change here: This line was already correct based on the previous error.
            // It correctly applies the Authorization Policy named "myschema" to the Hub.
            app.MapHub<ChatHub>("/chathub").RequireAuthorization("myschema");

            app.Run();
        }
    }
}