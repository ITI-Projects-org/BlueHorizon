using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Mappers;
using API.Models;
using API.UnitOfWorks;
using API.Hubs;
using Microsoft.Extensions.Options;
//using API.MapperConfig;

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

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 3;
            })
                .AddEntityFrameworkStores<BlueHorizonDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddOpenApi();
            builder.Services.AddSignalR();
            //builder.Services.AddAutoMapper(typeof(MappingConfig).Assembly);
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingConfig>());

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configure the HTTP request pipeline.
            builder.Services.AddAuthentication(options => {
                options.DefaultScheme = "myschema";          // for Authenticate, Challenge, Forbid
                options.DefaultAuthenticateScheme = "myschema";
                options.DefaultChallengeScheme = "myschema";
            })
            .AddJwtBearer("myschema", option =>
            {
                var key = "this is secrete key  for admin role base";
                var secreteKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = secreteKey,
                    NameClaimType = "nameid"
                };

                option.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // لو الطلب جاي من SignalR (WebSocket)
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/chathub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            }
            );
           

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
            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}
