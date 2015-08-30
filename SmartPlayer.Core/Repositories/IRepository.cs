using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartPlayer.Core.Repositories
{
    public interface IRepository
    {
        bool HasChanges();

        int SaveChanges();
    }

    public interface IRepository<TEntity> : IRepository where TEntity : class
    {
        void Add(IEnumerable<TEntity> entities);
        void Add(TEntity entity);
        int Update(TEntity entity, Expression<Func<TEntity, object>> changedProperty);
        void Create(IEnumerable<TEntity> entities);
        int Create(TEntity entity);
        IList<TEntity> GetAll();
        int Delete(IEnumerable<TEntity> entities);
        int Delete(TEntity entity);
        void DeleteWithoutSave(IEnumerable<TEntity> entities);
        void DeleteWithoutSave(TEntity entity);

        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);

    }
}
