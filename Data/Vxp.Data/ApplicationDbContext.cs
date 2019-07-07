﻿namespace Vxp.Data
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Vxp.Data.Common.Models;
    using Vxp.Data.Models;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Setting> Settings { get; set; }

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

        public override int SaveChanges() => this.SaveChanges(true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            this.SaveChangesAsync(true, cancellationToken);

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            ConfigureUserIdentityRelations(builder);

            EntityIndexesConfiguration.Configure(builder);

            var entityTypes = builder.Model.GetEntityTypes().ToList();

            // Set global query filter for not deleted entities only
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { builder });
            }

            // Disable cascade delete
            var foreignKeys = entityTypes
                .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private static void ConfigureUserIdentityRelations(ModelBuilder builder)
        {
            ConfigureUser(builder);

            ConfigureAddress(builder);

            ConfigureBankAccount(builder);

            ConfigureCompany(builder);

            ConfigureCountry(builder);

            ConfigureDistributorKey(builder);

            ConfigureDistributorUser(builder);

            ConfigureMessage(builder);

            ConfigureMessageRecipient(builder);

            ConfigureProduct(builder);

            ConfigureProductDetail(builder);

            ConfigureProject(builder);

            ConfigureDocument(builder);

            ConfigureOrder(builder);

            ConfigureOrderProduct(builder);

            ConfigurePriceModifier(builder);
        }

        private static void ConfigureOrderProduct(ModelBuilder builder)
        {
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
        }

        private static void ConfigurePriceModifier(ModelBuilder builder)
        {
            builder.Entity<PriceModifier>(entity =>
            {
                entity.Property(p => p.PriceModifierType).IsRequired();
                entity.Property(p => p.PriceModifierRange).IsRequired();
                entity.Property(p => p.PercentValue).IsRequired();
                entity.Property(p => p.Name).IsRequired();
            });
        }

        private static void ConfigureOrder(ModelBuilder builder)
        {
            builder.Entity<Order>(entity =>
            {
                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Orders)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }

        private static void ConfigureDocument(ModelBuilder builder)
        {
            builder.Entity<Document>(entity =>
            {
                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Documents)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.Location).IsRequired();
            });
        }

        private static void ConfigureProject(ModelBuilder builder)
        {
            builder.Entity<Project>(entity =>
            {
                entity.Property(p => p.Name).IsRequired();
            });
        }

        private static void ConfigureProductDetail(ModelBuilder builder)
        {
            builder.Entity<ProductDetail>(entity =>
            {
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Details)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureProduct(ModelBuilder builder)
        {
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
        }

        private static void ConfigureMessageRecipient(ModelBuilder builder)
        {
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
        }

        private static void ConfigureMessage(ModelBuilder builder)
        {
            builder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Topic)
                    .WithOne()
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(m => m.Body).IsRequired();
            });
        }

        private static void ConfigureDistributorUser(ModelBuilder builder)
        {
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
        }

        private static void ConfigureDistributorKey(ModelBuilder builder)
        {
            builder.Entity<DistributorKey>(e =>
            {
                e.HasOne(d => d.BankAccount)
                    .WithMany(b => b.DistributorKeys)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureCountry(ModelBuilder builder)
        {
            builder.Entity<Country>(e => e.HasKey(c => c.Id));
        }

        private static void ConfigureCompany(ModelBuilder builder)
        {
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
        }

        private static void ConfigureBankAccount(ModelBuilder builder)
        {
            builder.Entity<BankAccount>(entity =>
            {
                entity.HasMany(b => b.DistributorKeys)
                    .WithOne()
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(b => b.AccountNumber).IsRequired();
                entity.Property(b => b.BicCode).IsRequired();
                entity.Property(b => b.BankName).IsRequired();
            });
        }

        private static void ConfigureAddress(ModelBuilder builder)
        {
            builder.Entity<Address>(entity =>
            {
                entity.HasOne(a => a.Country);
            });
        }

        private static void ConfigureUser(ModelBuilder builder)
        {
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
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IDeletableEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        private void ApplyAuditInfoRules()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}