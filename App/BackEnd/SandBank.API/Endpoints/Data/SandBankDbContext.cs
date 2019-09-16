using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.MultiTenant;
using Domain;
using Domain.Account;
using Domain.Transaction;
using Domain.User;
using Endpoints.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Endpoints.Data
{
    public class SandBankDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<NumberRange> NumberRanges { get; set; }

        private readonly ITenantProvider _tenantProvider;
        private static readonly LoggerFactory ConsoleLoggerFactory =
            new LoggerFactory(new[]
            {
                new ConsoleLoggerProvider((category, level) =>
                    category == DbLoggerCategory.Database.Command.Name &&
                    level == LogLevel.Information, true)
            });

        public SandBankDbContext(DbContextOptions<SandBankDbContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuild)
        {
            optionsBuild.UseLoggerFactory(ConsoleLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.Name).Property<Guid>("ShadowId");
                modelBuilder.Entity(entityType.Name).ForNpgsqlUseXminAsConcurrencyToken();
            }
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Accounts)
                .WithOne(acc => acc.AccountOwner);

            modelBuilder.Entity<User>()
                .HasAlternateKey(user => user.Email);

            modelBuilder.Entity<Account>()
                .Property(acc => acc.AccountType)
                .HasConversion<string>();

            //need to generalize this 
            //add TenantId as a shadowProperty to every entity and auto add this to all DomainEntity classes.
            //add also set the value on creation
            modelBuilder.Entity<Account>().HasQueryFilter(acc => acc.AccountOwnerId == _tenantProvider.GetTenantId());

            modelBuilder.Entity<NumberRange>()
                .ToTable("NumberRanges");
                
            modelBuilder.Entity<NumberRange>()
                .Property(r => r.RangeType)
                .HasConversion<string>()
                .HasMaxLength(25)
                .IsRequired();

            modelBuilder.Entity<NumberRange>()
                .Property(r => r.Prefix)
                .IsRequired()
                .HasMaxLength(10);
            
            modelBuilder.Entity<NumberRange>()
                .Property(r => r.RangeStart)
                .IsRequired()
                .HasDefaultValue(1);
            
            modelBuilder.Entity<NumberRange>()
                .Property(r => r.RangeEnd)
                .IsRequired()
                .HasDefaultValue(999_999);
            
            modelBuilder.Entity<NumberRange>()
                .Property(r => r.LastValue)
                .IsRequired()
                .HasDefaultValue(0);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default (CancellationToken))
        {
            return await SaveChangesAsync(true, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.DetectChanges();

            foreach (var entity in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                entity.Property("ShadowId").CurrentValue = Guid.NewGuid();

                if (IsDomainEntity(entity))
                {
                    entity.Property("CreatedOn").CurrentValue = DateTime.UtcNow;
                }
            }
            
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private bool IsDomainEntity(EntityEntry entity)
        {
            var entityBaseType = entity.Metadata.ClrType.BaseType;
            var domainEntityRawType = typeof(DomainEntity<>);
            if (entityBaseType.IsConstructedGenericType && entityBaseType.GenericTypeArguments.Length == 1)
            {
                var domainEntityReifiedType = domainEntityRawType.MakeGenericType(entityBaseType.GenericTypeArguments[0]);
                if (entityBaseType == domainEntityReifiedType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}