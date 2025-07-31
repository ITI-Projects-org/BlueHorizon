using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

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

        // DbSets
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<AccessPermission> AccessPermissions { get; set; }
        public DbSet<Amenity> Amenity { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
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

            // -------------------------------------------------------------------
            // Fluent API and Relationships
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

            // Message relationships
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

            // OwnerVerificationDocument relationships
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
                .OnDelete(DeleteBehavior.Cascade);
                
            // AccessPermission relationships - cascade delete when booking is deleted
            builder.Entity<AccessPermission>()
                .HasOne(ap => ap.QRCode)
                .WithMany()
                .HasForeignKey(ap => ap.QRCodeId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<Unit>()
                .HasMany(u => u.UnitImagesTable)
                .WithOne(ui => ui.Unit)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}