using CodeLabX.EntityFramework.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodeLabX.EntityFramework.Data
{
    public class DataContext : DbContext, IDataContext
    {
        private IDbContextTransaction dbContextTransaction;

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public async Task SaveChangesAsync()
        {
            AddDefaultInfo();
            await base.SaveChangesAsync();
        }

        public int SaveChange()
        {
            AddDefaultInfo();
            return base.SaveChanges();
        }

        public void DeleteById<TEntity>(long? id) where TEntity : class
        {
            if (id == null) return;

            var entitySet = Set<TEntity>();
            var entity = entitySet.Find(id.Value);

            Entry(entity).State = EntityState.Deleted;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnDatabaseTableCreating(modelBuilder);
            OnResolveRelationship(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void AddDefaultInfo()
        {
            var entries = ChangeTracker.Entries().Where(d => d.Entity is EntityContext
                && (d.State == EntityState.Added || d.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((EntityContext)entry.Entity).CreatedDate = DateTimeOffset.UtcNow;
                }

                ((EntityContext)entry.Entity).ModifiedData = DateTimeOffset.UtcNow;
            }
        }

        private void OnDatabaseTableCreating(ModelBuilder modelBuilder)
        {
            DbContextHelper.CreateTable(modelBuilder);
        }

        private void OnResolveRelationship(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).OnDelete(DeleteBehavior.NoAction);
            //modelBuilder.Entity<Product>().HasOne(p => p.Supplier).WithMany(c => c.Products).OnDelete(DeleteBehavior.NoAction);
            //modelBuilder.Entity<Supplier>().HasOne(p => p.City).WithMany(c => c.Suppliers).OnDelete(DeleteBehavior.NoAction);
            //modelBuilder.Entity<Supplier>().HasMany(p => p.Products).WithOne(c => c.Supplier).HasForeignKey(s => s.SupplierId).OnDelete(DeleteBehavior.Cascade);
        }

        public void BeginTransaction() => dbContextTransaction = Database.BeginTransaction();
        public void CommitTransaction() => dbContextTransaction?.Commit();
        public void RollbackTransaction() => dbContextTransaction?.Rollback();
        public void DisposeTransaction() => dbContextTransaction?.Dispose();
    }
}
