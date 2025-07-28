using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public static class DataSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<BlueHorizonDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Seed Roles
            string[] roles = { "Admin", "Owner", "Tenant" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Admins
            var adminInfos = new[]
            {
                new { Name = "Ahmed Khaled", Email = "ahmedkhaled@bluehorizon.com" },
                new { Name = "Abdullah Mohamad", Email = "abdullahmohamad@bluehorizon.com" },
                new { Name = "Mark Magdy", Email = "markmagdy@bluehorizon.com" },
                new { Name = "Moomen Abdelshaheed", Email = "moomenabdelshaheed@bluehorizon.com" },
                new { Name = "Muhammad Sabbagh", Email = "muhammadsabbagh@bluehorizon.com" },
                new { Name = "Yousra Ehab", Email = "yousraehab@bluehorizon.com" }
            };
            foreach (var admin in adminInfos)
            {
                var userName = admin.Name.Replace(" ", "").ToLower();
                var existing = await userManager.FindByEmailAsync(admin.Email);
                if (existing == null)
                {
                    var adminUser = new Admin
                    {
                        UserName = userName,
                        Email = admin.Email,
                        EmailConfirmed = true,
                        RegistrationDate = DateTime.Now.AddDays(-5),
                        UserType = "Admin",
                        PhoneNumber = new Random().Next(100000000, 999999999),
                        PhoneNumberConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        NormalizedUserName = userName.ToUpper(),
                        NormalizedEmail = admin.Email.ToUpper()
                    };
                    await userManager.CreateAsync(adminUser, "Admin@123");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 3. Seed Owners
            string[] ownerNames = {
                "Ahmed Mansour", "Fatma Salah", "Mohamed Sami", "Sara Hany", "Khaled Wagdy",
                "Nour Emad", "Tarek Gamal", "Hala Sherif", "Youssef Nabil", "Mona Atef"
            };
            foreach (var ownerName in ownerNames)
            {
                var userName = ownerName.Replace(" ", "").ToLower();
                var email = $"{userName}@example.com";
                var existing = await userManager.FindByEmailAsync(email);
                if (existing == null)
                {
                    var ownerUser = new Owner
                    {
                        UserName = userName,
                        Email = email,
                        EmailConfirmed = true,
                        RegistrationDate = DateTime.Now.AddDays(-30),
                        UserType = "Owner",
                        PhoneNumber = new Random().Next(100000000, 999999999),
                        PhoneNumberConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        NormalizedUserName = userName.ToUpper(),
                        NormalizedEmail = email.ToUpper(),
                        BankAccountDetails = $"Bank Account {new Random().Next(1000, 9999)}",
                        VerificationStatus = VerificationStatus.Verified,
                        VerificationDate = DateTime.Now.AddDays(-5),
                        VerificationNotes = "Owner data seeded for testing."
                    };
                    await userManager.CreateAsync(ownerUser, "Password123!");
                    await userManager.AddToRoleAsync(ownerUser, "Owner");
                }
            }

            // 4. Seed Amenities
            if (!context.Amenity.Any())
            {
                var amenitiesToSeed = new List<Amenity>
                {
                    new Amenity { Name = AmenityName.AC },
                    new Amenity { Name = AmenityName.PoolAccess },
                    new Amenity { Name = AmenityName.WIFI }
                };
                context.Amenity.AddRange(amenitiesToSeed);
                await context.SaveChangesAsync(); // Save to get generated Ids
            }

            // 5. Seed Units, UnitAmenities, and UnitImages
            if (!context.Units.Any())
            {
                var random = new Random();
                var owners = context.Owners.ToList();
                var amenities = context.Amenity.ToList();
                var unitsToSeed = new List<Unit>();
                var allUnitAmenitiesToSeed = new List<UnitAmenity>();
                var allUnitImagesToSeed = new List<UnitImages>();

                string[] villageNames = { "North Coast", "Ain Sokhna", "El Gouna", "Marsa Alam", "Hurghada", "Sharm El Sheikh" };
                string[] apartmentTitles = { "Cozy Apartment", "Modern Flat", "Spacious Studio", "Luxury Penthouse", "Sea View Apartment" };
                string[] chaletTitles = { "Beachfront Chalet", "Garden Chalet", "Family Chalet", "Relaxing Chalet", "Poolside Chalet" };
                string[] villaTitles = { "Grand Villa", "Private Villa", "Luxury Villa", "Estate Villa", "Mountain View Villa" };
                string[] descriptions = {
                    "A beautiful unit perfect for family vacations.",
                    "Enjoy stunning views and comfortable living.",
                    "Ideal for a relaxing getaway near the sea.",
                    "Modern design with all essential amenities.",
                    "Spacious and well-equipped for a memorable stay."
                };

                for (int i = 0; i < 100; i++)
                {
                    var owner = owners[random.Next(owners.Count)];
                    var unitType = (UnitType)random.Next(Enum.GetNames(typeof(UnitType)).Length);
                    string title;
                    switch (unitType)
                    {
                        case UnitType.Apartment:
                            title = apartmentTitles[random.Next(apartmentTitles.Length)];
                            break;
                        case UnitType.Chalet:
                            title = chaletTitles[random.Next(chaletTitles.Length)];
                            break;
                        case UnitType.Villa:
                            title = villaTitles[random.Next(villaTitles.Length)];
                            break;
                        default:
                            title = "Holiday Unit";
                            break;
                    }

                    unitsToSeed.Add(new Unit
                    {
                        OwnerId = owner.Id,
                        Title = title,
                        Description = descriptions[random.Next(descriptions.Length)],
                        UnitType = unitType,
                        Bedrooms = random.Next(1, 5),
                        Bathrooms = random.Next(1, 4),
                        Sleeps = random.Next(2, 10),
                        DistanceToSea = random.Next(50, 5000),
                        BasePricePerNight = Math.Round((decimal)(random.Next(500, 10000) + random.NextDouble()), 2),
                        Address = $"Street {random.Next(1, 100)}, Building {random.Next(1, 50)}",
                        VillageName = villageNames[random.Next(villageNames.Length)],
                        CreationDate = DateTime.Now.AddDays(-random.Next(1, 365)),
                        AverageUnitRating = (float)Math.Round(random.NextDouble() * 5, 1),
                        VerificationStatus = VerificationStatus.Pending,
                        Contract = (DocumentType)random.Next(Enum.GetNames(typeof(DocumentType)).Length),
                        ContractPath = "https://imgv2-2-f.scribdassets.com/img/document/505704548/original/091be6f67a/1?v=1"
                    });
                }

                context.Units.AddRange(unitsToSeed);
                await context.SaveChangesAsync(); // Save to get generated Unit Ids

                var allUnits = context.Units.ToList();
                var amenityIds = context.Amenity.Select(a => a.Id).ToList();
                foreach (var unit in allUnits)
                {
                    foreach (var amenityId in amenityIds)
                    {
                        allUnitAmenitiesToSeed.Add(new UnitAmenity { UnitId = unit.Id, AmenityId = amenityId });
                    }
                }
                context.UnitAmenities.AddRange(allUnitAmenitiesToSeed);
                context.UnitImages.AddRange(allUnitImagesToSeed);
            }

            await context.SaveChangesAsync();
        }
    }
}
