using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vxp.Data.Models;

namespace Vxp.App.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<PriceModifier> PriceModifiers { get; set; }
        public DbSet<DistributorKey> DistributorKeys { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Address>(entity =>
            {
                entity.HasOne(a => a.Country);
            });

            builder.Entity<BankAccount>(entity =>
            {
                entity.HasMany(b => b.DistributorKeys)
                    .WithOne()
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(b => b.AccounNumber).IsRequired();
                entity.Property(b => b.BicCode).IsRequired();
                entity.Property(b => b.BankName).IsRequired();
            });

            builder.Entity<Company>(entity =>
            {
                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.BusinessNumber).IsRequired();

                entity.HasMany(c => c.Members)
                    .WithOne(u => u.Company)
                    .HasForeignKey(u => u.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.ShippingAddress);
                entity.HasOne(c => c.ContactAddress);
            });


            builder.Entity<Country>(e => e.HasKey(c => c.Id));

            builder.Entity<DistributorKey>(e =>
            {
                e.HasOne(d => d.BankAccount)
                     .WithMany(b => b.DistributorKeys)
                     .IsRequired()
                     .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<DistributorUser>(entity =>
            {
                entity.HasKey(k => new { k.ApplicationUserId, k.DistributorKeyId });

                entity.HasOne(e => e.DistributorKey)
                    .WithMany(k => k.Customers)
                    .HasForeignKey(e => e.DistributorKeyId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ApplicationUser)
                    .WithMany(k => k.Distributors)
                    .HasForeignKey(e => e.ApplicationUserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Roles)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.BankAccounts)
                    .WithOne(a => a.Owner)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Projects)
                    .WithOne(p => p.Owner)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.PriceModifiersReceive)
                    .WithOne(p => p.Buyer)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.PriceModifiersGive)
                    .WithOne(p => p.Seller)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Topic)
                    .WithOne()
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(m => m.Body).IsRequired();
            });

            builder.Entity<MessageRecipient>(entity =>
            {
                entity.HasKey(k => new { k.MessageId, k.RecipientId });

                entity.HasOne(m => m.Message)
                    .WithMany(m => m.Recipients)
                    .HasForeignKey(m => m.MessageId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Recipient)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(m => m.RecipientId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Images)
                    .WithOne()
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Image);

            });

            builder.Entity<ProductDetail>(entity =>
            {
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Details)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Project>(entity =>
            {
                entity.Property(p => p.Name).IsRequired();
            });

            builder.Entity<Document>(entity =>
            {
                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Documents)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.Location).IsRequired();
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Orders)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<PriceModifier>(entity =>
            {
                entity.Property(p => p.PriceModifierType).IsRequired();
                entity.Property(p => p.PriceModifierRange).IsRequired();
                entity.Property(p => p.PercentValue).IsRequired();
                entity.Property(p => p.Name).IsRequired();

            });

            builder.Entity<OrderProduct>(entity =>
            {
                entity.HasOne(o => o.Order)
                    .WithMany(o => o.Products)
                    .HasForeignKey(o => o.OrderId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Product)
                    .WithMany(o => o.Orders)
                    .HasForeignKey(o => o.ProductId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });


            base.OnModelCreating(builder);
        }
    }
}
