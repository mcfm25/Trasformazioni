using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Interceptors
{
    /// <summary>
    /// Interceptor per popolare automaticamente i campi di audit (CreatedAt, CreatedBy, ModifiedAt, ModifiedBy)
    /// quando le entità vengono salvate nel database.
    /// </summary>
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateAuditFields(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditFields(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditFields(DbContext? context)
        {
            if (context == null) return;

            var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var now = DateTime.Now;

            var entries = context.ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        // Previeni la modifica dei campi di creazione
                        entry.Property(e => e.CreatedAt).IsModified = false;
                        entry.Property(e => e.CreatedBy).IsModified = false;

                        // Aggiorna i campi di modifica solo se non è un soft delete
                        if (!entry.Entity.IsDeleted)
                        {
                            entry.Entity.ModifiedAt = now;
                            entry.Entity.ModifiedBy = userId;
                        }
                        break;

                    case EntityState.Deleted:
                        // Implementa soft delete: invece di cancellare, marca come deleted
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = now;
                        entry.Entity.DeletedBy = userId;
                        break;
                }
            }


            // Fix per ApplicationUser che implementa IAuditableEntity
            var userEntries = context.ChangeTracker.Entries<IAuditableEntity>();

            foreach (var entry in userEntries)
            {
                if (entry.Entity is ApplicationUser)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatedAt = now;
                            entry.Entity.CreatedBy = userId;
                            entry.Entity.IsDeleted = false;
                            break;

                        case EntityState.Modified:
                            // Previeni la modifica dei campi di creazione
                            entry.Property(e => e.CreatedAt).IsModified = false;
                            entry.Property(e => e.CreatedBy).IsModified = false;

                            if (!entry.Entity.IsDeleted)
                            {
                                entry.Entity.ModifiedAt = now;
                                entry.Entity.ModifiedBy = userId;
                            }
                            break;

                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entry.Entity.IsDeleted = true;
                            entry.Entity.DeletedAt = now;
                            entry.Entity.DeletedBy = userId;
                            break;
                    }
                }
            }
        }
    }
}