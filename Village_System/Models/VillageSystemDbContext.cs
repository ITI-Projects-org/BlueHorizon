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
        public DbSet<Amenity> Amenty { get; set; }
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
            builder.Entity<ApplicationUser>()
                .HasDiscriminator<string>("UserType")
                .HasValue<ApplicationUser>("Base")
                .HasValue<Tenant>("Tenant")
                .HasValue<Owner>("Owner");
            // .HasValue<Admin>("Admin");

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<Booking>()
            .Property(b=>b.TotalPrice)
            .HasPrecision(10,2);
            builder.Entity<Booking>()
            .Property(b=>b.PlatformComission)
            .HasPrecision(10,2);
            builder.Entity<Booking>()
            .Property(b=>b.OwnerPayoutAmount)
            .HasPrecision(10,2);
        }
    }
}
