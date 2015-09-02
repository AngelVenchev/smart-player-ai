using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using SmartPlayer.Data;

namespace SmartPlayer.Core.Repositories
{
    public abstract class BaseRepository : IRepository
    {
        private SmartPlayerEntities _context;

        public SmartPlayerEntities Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public BaseRepository(SmartPlayerEntities context)
        {
            _context = context;
        }


		public virtual bool HasChanges()
		{
			return this.Context.ChangeTracker.HasChanges();
		}
		
		public virtual int SaveChanges()
        {
            return this.Context.SaveChanges();
        }
    }

    public abstract class BaseRepository<TEntity> : BaseRepository, IRepository<TEntity> where TEntity : class
    {
        public BaseRepository(SmartPlayerEntities context)
            : base(context) { }

        private DbSet<TEntity> objectSet;

        private DbSet<TEntity> ObjectSet
        {
            get
            {
                if (this.objectSet == null)
                {
                    this.objectSet = this.Context.Set<TEntity>();
                }

                return this.objectSet;
            }
        }

        public virtual void Add(TEntity entity)
        {
            this.ObjectSet.Add(entity);
        }

        public virtual void Add(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.ObjectSet.Add(entity);
            }
        }

        public virtual void Attach(TEntity entity)
        {
            var localEntity = this.ObjectSet.Local.FirstOrDefault();
            this.ObjectSet.Attach(entity);
        }

        public virtual void Attach(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.ObjectSet.Attach(entity);
            }
        }


        public virtual int Update(TEntity entity, Expression<Func<TEntity, object>> changedProperty)
        {
            this.Attach(entity);
            this.Context.Entry(entity).Property(changedProperty).IsModified = true;
            return this.Context.SaveChanges();
        }

        public virtual int Create(TEntity entity)
        {
            this.ObjectSet.Add(entity);

            return this.SaveChanges();
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return this.ObjectSet.Where(predicate);
        }


        public virtual void Create(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.ObjectSet.Add(entity);
            }

            this.SaveChanges();
        }

        //public IList<TEntity> GetAll()
        //{
        //    return this.ObjectSet.ToList();
        //}

        public IQueryable<TEntity> GetAll()
        {
            return this.ObjectSet;
        }
        public int Delete(TEntity entity)
        {
            DeleteWithoutSave(entity);
            return this.SaveChanges();
        }

        public int Delete(IEnumerable<TEntity> entities)
        {
            DeleteWithoutSave(entities);
            return this.SaveChanges();
        }

        public void DeleteWithoutSave(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DeleteWithoutSave(entity);
            }
        }

        public virtual void DeleteWithoutSave(TEntity entity)
        {
            this.ObjectSet.Remove(entity);
        }

    }
}
