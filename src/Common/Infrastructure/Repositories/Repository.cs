using Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public interface IRepository<TEntity>
    {
        #region Get
        IQueryable<TEntity> GetQuery();
        IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> expression);
        DbContext GetDbContext();

        TEntity Single(Expression<Func<TEntity, bool>> criteria);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> criteria);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> criteria);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> criteria);


        #endregion

        #region Find
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> criteria);
        TEntity FindOne(Expression<Func<TEntity, bool>> criteria);
        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> criteria);
        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>>[] includeExpressions);
        #endregion

        #region Count

        int Count();
        Task<int> CountAsync();
        int Count(Expression<Func<TEntity, bool>> criteria);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> criteria);
        Task<bool> GetAny(Expression<Func<TEntity, bool>> expression);
        #endregion

        #region Update
        void Update(TEntity entity, Expression<Func<TEntity, bool>> criteria);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        #endregion

        #region Add
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        #endregion

        #region Remove
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        #endregion

        #region Sql
        #endregion
        // Get the lastest value from db
        void Reload(TEntity entity);

    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ExampleDbContext Context;
        private readonly Type _type;

        public Repository(ExampleDbContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
            _type = typeof(TEntity);
        }


        protected DbSet<TEntity> DbSet { get; }

        public void Update(TEntity entity, Expression<Func<TEntity, bool>> criteria)
        {
            var original = FindOne(criteria);
            Context.Entry(original).CurrentValues.SetValues(entity);
        }

        public void Update(TEntity entity)
        {
            Context.Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            Context.UpdateRange(entities);
        }

        public void Add(TEntity entity)
        {
            if (entity != null) DbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().CountAsync(criteria);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        public IQueryable<TEntity> GetQuery()
        {
            return DbSet;
        }

        public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> expression)
        {
            return Context.Set<TEntity>().Where(expression);
        }

        public async Task<bool> GetAny(Expression<Func<TEntity, bool>> expression)
        {
            return await Context.Set<TEntity>().AnyAsync(expression);
        }


        public DbContext GetDbContext()
        {
            return Context;
        }

        public TEntity Single(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().Single(criteria);
        }

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().SingleAsync(criteria);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().SingleOrDefault(criteria);
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().SingleOrDefaultAsync(criteria);
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().Where(criteria);
        }

        public TEntity FindOne(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().Where(criteria).FirstOrDefault();
        }

        public Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().Where(criteria).FirstOrDefaultAsync();
        }

        public Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>>[] includeExpressions)
        {
            IQueryable<TEntity> includeQueryable = DbSet;
            foreach (var includeExpression in includeExpressions)
            {
                includeQueryable = includeQueryable.Include(includeExpression);
            }

            return includeQueryable.FirstOrDefaultAsync(expression);
        }

        public int Count()
        {
            return GetQuery().Count();
        }

        public Task<int> CountAsync()
        {
            return GetQuery().CountAsync();
        }

        public int Count(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().Count(criteria);
        }

        public void Remove(TEntity entity)
        {
            if (entity != null) DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public void Reload(TEntity entity)
        {
            Context.Entry(entity).Reload();
        }

        /// <summary>
        /// Check model have a specific property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected bool HasProperty(string property)
        {
            return _type.GetProperty(property) != null;
        }
        protected void SetProperty(TEntity entity, string property, object value)
        {
            if (value != null)
            {
                var parseValue = Guid.TryParse(value.ToString(), out var guid) ? guid : value;
                entity.GetType().GetProperty(property).SetValue(entity, parseValue);
            }
        }
    }
}
