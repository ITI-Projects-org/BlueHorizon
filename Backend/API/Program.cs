using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Mappers;
using API.Models;
using API.UnitOfWorks;
using API.Services.Interfaces;
using API.Services.Implementation;
using Microsoft.AspNetCore.Identity.UI.Services;
using API.Repositories.Interfaces;
using API.Repositories.Implementations;
//using API.MapperConfig;

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
            builder.Services.AddDbContext<VillageSystemDbContext>(options => 
            options.UseLazyLoadingProxies()
            .UseSqlServer(builder.Configuration.GetConnectionString("cs")));
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireUppercase= false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase=false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase= false;
                options.Password.RequiredLength= 3;
            })
                .AddEntityFrameworkStores<VillageSystemDbContext>()
                .AddDefaultTokenProviders();
            
            builder.Services.AddOpenApi();
            //builder.Services.AddAutoMapper(typeof(MappingConfig).Assembly);
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingConfig>());
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();


            // Configure the HTTP request pipeline.
            builder.Services.AddAuthentication(op => op.DefaultAuthenticateScheme="myschema")
            .AddJwtBearer("myschema", option =>
            {
                var key = builder.Configuration["JwtKey"]!;
                var secreteKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = secreteKey
                };
            }  
            );

            builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:4200") // <-- your frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
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
            app.Run();
        }
    }
}
