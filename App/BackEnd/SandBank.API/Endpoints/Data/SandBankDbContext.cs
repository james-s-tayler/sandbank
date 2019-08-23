using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Domain;
using Microsoft.AspNetCore.SignalR.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Endpoints.Data
{
    public class SandBankDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public SandBankDbContext(DbContextOptions<SandBankDbContext> options)
            : base(options)
        {    
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.Name).Property<Guid>("ShadowId");
                modelBuilder.Entity(entityType.Name).ForNpgsqlUseXminAsConcurrencyToken();
            }
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