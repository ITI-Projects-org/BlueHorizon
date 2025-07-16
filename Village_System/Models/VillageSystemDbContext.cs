using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Village_System.Models
{
    public class VillageSystemDbContext : IdentityDbContext<ApplicationUser>
    {
        public VillageSystemDbContext(DbContextOptions<VillageSystemDbContext> options) : base(options) { }

        public DbSet<Owner> Owners { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<AccessPermission> AccessPermission{ get; set; }
        public DbSet<Amenity> Amenity { get; set; }
        public DbSet<Booking> Bookings{ get; set; }
        public DbSet<Message> Messages{ get; set; }
        public DbSet<OwnerReview> OwnerReviews{ get; set; }
        public DbSet<OwnerVerificationDocument> OwnerVerificationDocuments{ get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<QRCode> QRCodes{ get; set; }
        public DbSet<UnitAmenity> UnitAmenities{ get; set; }
        public DbSet<UnitReview> UnitReviews{ get; set; }
    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Unit>()
                .Property(u => u.BasePricePerNight)
                .HasPrecision(10, 2);

            

            builder.Entity<ApplicationUser>()
                .HasDiscriminator<string>("UserType")
                .HasValue<ApplicationUser>("Base")
                .HasValue<Tenant>("Tenant")
                .HasValue<Owner>("Owner");

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

            // UnitReview relationships
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

            // OwnerReview relationships
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

            // Booking relationships
            builder.Entity<Booking>()
                .HasOne(b => b.Tenant)
                .WithMany()
                .HasForeignKey(b => b.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Unit)
                .WithMany()
                .HasForeignKey(b => b.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unit relationships
            builder.Entity<Unit>()
                .HasOne(u => u.Owner)
                .WithMany()
                .HasForeignKey(u => u.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message relationships
            builder.Entity<Message>()
                .HasOne(m => m.SenderUser)
                .WithMany()
                .HasForeignKey(m => m.Sender)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.RecieverUser)
                .WithMany()
                .HasForeignKey(m => m.Reciever)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Booking)
                .WithMany()
                .HasForeignKey(m => m.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // OwnerVerificationDocument relationships
            builder.Entity<OwnerVerificationDocument>()
                .HasOne(o => o.Owner)
                .WithMany()
                .HasForeignKey(o => o.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UnitAmenity>()
                .HasOne(ua => ua.Unit)
                .WithMany(u => u.UnitAmenities)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<UnitAmenity>()
                .HasOne(ua => ua.Amenity)
                .WithMany(a => a.UnitAmenities)
                .OnDelete( DeleteBehavior.Restrict);
            builder.Entity<QRCode>()
                .HasOne(QRCode => QRCode.Booking)
                .WithOne(b => b.QRCode)
                .HasForeignKey<QRCode>(qr => qr.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
                ;

        }
    }
}
