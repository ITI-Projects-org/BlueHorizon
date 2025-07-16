
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Village_System.Mappers;
using Village_System.Models;
//using Village_System.MapperConfig;

namespace Village_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

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




            // Configure the HTTP request pipeline.
            builder.Services.AddAuthentication(op => op.DefaultAuthenticateScheme="myschema")
            .AddJwtBearer("myschema", option =>
            {
                var key = "this is secrete key  for admin role base";
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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
