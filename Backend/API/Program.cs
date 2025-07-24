using API.Hubs;
using API.Mappers;
using API.Models;
using API.Services.Implementation;
using API.Services.Interfaces;
using API.UnitOfWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
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
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingConfig>());

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddTransient<IEmailSender, EmailSender>();


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "myschema";          
                options.DefaultAuthenticateScheme = "myschema";
                options.DefaultChallengeScheme = "myschema";
            })
            .AddJwtBearer("myschema", options =>
            {
                var key = builder.Configuration["JwtKey"]!;
                var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = secretKey
                };

                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
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
            })
            .AddGoogle("Google", options =>
            {
                options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
                options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
                options.CallbackPath = "/signin-google";
                options.SaveTokens = true;
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
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }
            app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));

            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.MapHub<ChatHub>("/chathub").RequireAuthorization("myschema");

            app.Run();
        }
    }
}