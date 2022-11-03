using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    interface IUsageContext : IDisposable
    {
        int SaveChanges();

        void Add<TEntity>(TEntity item) where TEntity : BaseModels;
        ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>([NotNullAttribute] TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseModels;

        void AddRange<TEntity>(List<TEntity> data) where TEntity : BaseModels;

        void SaveRange<TEntity>(List<TEntity> data) where TEntity : BaseModels;

        void Save(BaseModels item);

        void Sync(BaseModels item);

        void DeleteRange<TEntity>(List<TEntity> data) where TEntity : BaseModels;

        void Delete(BaseModels item);

        void DeleteNotUpdate<TEntity>(TEntity item) where TEntity : BaseModels;

        void BeginTransaction();

        void Commit();

        void Rollback();

    }
}
