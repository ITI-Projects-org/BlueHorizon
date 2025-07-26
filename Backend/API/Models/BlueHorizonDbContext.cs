using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Microsoft.EntityFrameworkCore.Diagnostics; // Required for suppressing the warning

namespace API.Models
{
    public class BlueHorizonDbContext : IdentityDbContext<ApplicationUser>
    {
        public BlueHorizonDbContext(DbContextOptions<BlueHorizonDbContext> options) : base(options) { }

        // Method to suppress the dynamic data warning (keep this!)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        // DbSets (should match your actual models)
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<AccessPermission> AccessPermissions { get; set; }
        public DbSet<Amenity> Amenity { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<OwnerReview> OwnerReviews { get; set; }
        public DbSet<OwnerVerificationDocument> OwnerVerificationDocuments { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<QRCode> QRCodes { get; set; }
        public DbSet<UnitAmenity> UnitAmenities { get; set; }
        public DbSet<UnitReview> UnitReviews { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UnitImages> UnitImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //// -------------------------------------------------------------------
            //// Data Seed Code
            //// -------------------------------------------------------------------

            //var random = new Random();

            //// 1. Seed Owners
            //var ownersToSeed = new List<Owner>();
            //string[] egyptianNames = {
            //    "Ahmed Mansour", "Fatma Salah", "Mohamed Sami", "Sara Hany", "Khaled Wagdy",
            //    "Nour Emad", "Tarek Gamal", "Hala Sherif", "Youssef Nabil", "Mona Atef"
            //};

            //for (int i = 0; i < 10; i++)
            //{
            //    var ownerId = Guid.NewGuid().ToString();
            //    ownersToSeed.Add(new Owner
            //    {
            //        Id = ownerId,
            //        UserName = egyptianNames[i % egyptianNames.Length].Replace(" ", ""),
            //        NormalizedUserName = egyptianNames[i % egyptianNames.Length].Replace(" ", "").ToUpper(),
            //        Email = $"{egyptianNames[i % egyptianNames.Length].Replace(" ", "").ToLower()}@example.com",
            //        NormalizedEmail = $"{egyptianNames[i % egyptianNames.Length].Replace(" ", "").ToLower()}@example.com".ToUpper(),
            //        EmailConfirmed = true,
            //        PhoneNumber = random.Next(100000000, 999999999),
            //        PhoneNumberConfirmed = true,
            //        SecurityStamp = Guid.NewGuid().ToString(),
            //        ConcurrencyStamp = Guid.NewGuid().ToString(),
            //        PasswordHash = "AQAAAAIAAYagAAAAEP0wI0fW+pT+uJ7R9zDq5E9y2l1z+J7P1r2S8L7D0S5S7Q9x2k0m3r4v5w6x7y8z9A+B=", // Dummy hash for "Password123!"
            //        RegistrationDate = DateTime.Now.AddDays(-random.Next(30, 730)),
            //        UserType = "Owner",
            //        BankAccountDetails = $"Bank Account {random.Next(1000, 9999)}",
            //        VerificationStatus = VerificationStatus.Verified,
            //        VerificationDate = DateTime.Now.AddDays(-random.Next(1, 30)),
            //        VerificationNotes = "Owner data seeded for testing."
            //    });
            //}
            //builder.Entity<Owner>().HasData(ownersToSeed);

            //// 2. Seed Amenities - ONLY using values from your defined AmenityName enum
            //var amenitiesToSeed = new List<Amenity>
            //{
            //    new Amenity { Id = 1, Name = AmenityName.AC },
            //    new Amenity { Id = 2, Name = AmenityName.PoolAccess },
            //    new Amenity { Id = 3, Name = AmenityName.WIFI }
            //    // Removed: AmenityName.Balcony, AmenityName.Kitchen, AmenityName.Parking
            //    // as they caused the "Requested value was not found" error.
            //};
            //builder.Entity<Amenity>().HasData(amenitiesToSeed);

            //// 3. Seed Units, UnitAmenities, and UnitImages
            //var unitsToSeed = new List<Unit>();
            //var allUnitAmenitiesToSeed = new List<UnitAmenity>();
            //var allUnitImagesToSeed = new List<UnitImages>();

            //string[] villageNames = { "North Coast", "Ain Sokhna", "El Gouna", "Marsa Alam", "Hurghada", "Sharm El Sheikh" };
            //string[] apartmentTitles = { "Cozy Apartment", "Modern Flat", "Spacious Studio", "Luxury Penthouse", "Sea View Apartment" };
            //string[] chaletTitles = { "Beachfront Chalet", "Garden Chalet", "Family Chalet", "Relaxing Chalet", "Poolside Chalet" };
            //string[] villaTitles = { "Grand Villa", "Private Villa", "Luxury Villa", "Estate Villa", "Mountain View Villa" };
            //string[] descriptions = {
            //    "A beautiful unit perfect for family vacations.",
            //    "Enjoy stunning views and comfortable living.",
            //    "Ideal for a relaxing getaway near the sea.",
            //    "Modern design with all essential amenities.",
            //    "Spacious and well-equipped for a memorable stay."
            //};

            //for (int i = 0; i < 100; i++)
            //{
            //    var owner = ownersToSeed[random.Next(ownersToSeed.Count)];
            //    var unitType = (UnitType)random.Next(Enum.GetNames(typeof(UnitType)).Length);
            //    string title;
            //    string imageCategory;

            //    switch (unitType)
            //    {
            //        case UnitType.Apartment:
            //            title = apartmentTitles[random.Next(apartmentTitles.Length)];
            //            imageCategory = "apartment";
            //            break;
            //        case UnitType.Chalet:
            //            title = chaletTitles[random.Next(chaletTitles.Length)];
            //            imageCategory = "beach chalet";
            //            break;
            //        case UnitType.Villa:
            //            title = villaTitles[random.Next(villaTitles.Length)];
            //            imageCategory = "luxury villa";
            //            break;
            //        default:
            //            title = "Holiday Unit";
            //            imageCategory = "house";
            //            break;
            //    }

            //    var unitId = i + 1;

            //    unitsToSeed.Add(new Unit
            //    {
            //        Id = unitId,
            //        OwnerId = owner.Id,
            //        Title = title,
            //        Description = descriptions[random.Next(descriptions.Length)],
            //        UnitType = unitType,
            //        Bedrooms = random.Next(1, 5),
            //        Bathrooms = random.Next(1, 4),
            //        Sleeps = random.Next(2, 10),
            //        DistanceToSea = random.Next(50, 5000),
            //        BasePricePerNight = Math.Round((decimal)(random.Next(500, 10000) + random.NextDouble()), 2),
            //        Address = $"Street {random.Next(1, 100)}, Building {random.Next(1, 50)}",
            //        VillageName = villageNames[random.Next(villageNames.Length)],
            //        CreationDate = DateTime.Now.AddDays(-random.Next(1, 365)),
            //        AverageUnitRating = (float)Math.Round(random.NextDouble() * 5, 1),
            //        VerificationStatus = VerificationStatus.Pending,
            //        Contract = (DocumentType)random.Next(Enum.GetNames(typeof(DocumentType)).Length),
            //        ContractPath = $"https://example.com/contracts/{Guid.NewGuid()}.pdf",
            //    });

            //    // Add UnitAmenities - ONLY using AmenityIds that exist in the seeded Amenity table
            //    allUnitAmenitiesToSeed.Add(new UnitAmenity { UnitId = unitId, AmenityId = 1 }); // AC
            //    allUnitAmenitiesToSeed.Add(new UnitAmenity { UnitId = unitId, AmenityId = 2 }); // PoolAccess
            //    allUnitAmenitiesToSeed.Add(new UnitAmenity { UnitId = unitId, AmenityId = 3 }); // WIFI
            //    // Removed random additions for Balcony, Kitchen, Parking if they are not in your AmenityName enum
            //}

            //builder.Entity<Unit>().HasData(unitsToSeed);
            //builder.Entity<UnitAmenity>().HasData(allUnitAmenitiesToSeed);
            //builder.Entity<UnitImages>().HasData(allUnitImagesToSeed);

            // -------------------------------------------------------------------
            // Fluent API and Relationships (unchanged)
            // -------------------------------------------------------------------

            builder.Entity<Unit>()
                .Property(u => u.BasePricePerNight)
                .HasPrecision(10, 2);

            builder.Entity<Message>().Property(m => m.SenderId).IsRequired();
            builder.Entity<Message>().Property(m => m.ReceiverId).IsRequired();
            builder.Entity<Message>().Property(m => m.MessageContent).IsRequired();

            builder.Entity<Unit>()
            .HasMany(u => u.UnitAmenities)
            .WithOne(ua => ua.Unit)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Unit>()
                .HasOne(u => u.Owner)
                .WithMany(o => o.Units)
                .HasForeignKey(u => u.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasDiscriminator<string>("UserType")
                .HasValue<ApplicationUser>("Base")
                .HasValue<Tenant>("Tenant")
                .HasValue<Owner>("Owner")
                .HasValue<Admin>("Admin");

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(10, 2);
            builder.Entity<Booking>()
                .Property(b => b.PlatformComission)
                .HasPrecision(10, 2);
            builder.Entity<Booking>()
                .Property(b => b.OwnerPayoutAmount)
                .HasPrecision(10, 2);

            builder.Entity<UnitReview>()
                .HasOne(u => u.Tenant)
                .WithMany()
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UnitReview>()
                .HasOne(u => u.Unit)
                .WithMany()
                .HasForeignKey(u => u.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UnitReview>()
                .HasOne(u => u.Booking)
                .WithMany()
                .HasForeignKey(u => u.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OwnerReview>()
                .HasOne(o => o.Owner)
                .WithMany()
                .HasForeignKey(o => o.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OwnerReview>()
                .HasOne(o => o.Tenant)
                .WithMany()
                .HasForeignKey(o => o.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OwnerReview>()
                .HasOne(o => o.Booking)
                .WithMany()
                .HasForeignKey(o => o.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Tenant)
                .WithMany()
                .HasForeignKey(b => b.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Unit)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.SenderUser)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.ReceiverUser)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OwnerVerificationDocument>()
                .HasOne(o => o.Owner)
                .WithMany(o => o.OwnerVerificationDocuments)
                .HasForeignKey(o => o.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UnitAmenity>()
                .HasOne(ua => ua.Unit)
                .WithMany(u => u.UnitAmenities)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UnitAmenity>()
                .HasOne(ua => ua.Amenity)
                .WithMany(a => a.UnitAmenities)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QRCode>()
                .HasOne(QRCode => QRCode.Booking)
                .WithOne(b => b.QRCode)
                .HasForeignKey<QRCode>(qr => qr.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Unit>()
                .HasMany(u => u.UnitImagesTable)
                .WithOne(ui => ui.Unit)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}