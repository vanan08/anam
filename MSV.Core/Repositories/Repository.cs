using Accuracy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accuracy.Core.Repositories
{
    public class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        private NORTHWNDContext Context { get; set; }

        public Repository()
        {
            this.Context = new NORTHWNDContext();
        }

        public void Save(TModel instance)
        {
            Context.Set<TModel>().Add(instance);
            Context.Entry(instance).State = System.Data.Entity.EntityState.Added;
            Context.SaveChanges();
        }

        public void Update(TModel instance)
        {
            Context.Set<TModel>().Attach(instance);
            Context.Entry(instance).State = System.Data.Entity.EntityState.Modified;
            Context.SaveChanges();
        }

        public void Delete(TModel instance)
        {
            Context.Set<TModel>().Remove(instance);
            Context.SaveChanges();
        }

        public async Task SaveAsync(TModel instance)
        {
            Context.Set<TModel>().Add(instance);
            Context.Entry(instance).State = System.Data.Entity.EntityState.Added;
            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TModel instance)
        {
            Context.Set<TModel>().Attach(instance);
            Context.Entry(instance).State = System.Data.Entity.EntityState.Modified;
            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TModel instance)
        {
            Context.Set<TModel>().Remove(instance);
            await Context.SaveChangesAsync();
        }

        public IQueryable<TModel> FindAll()
        {
            return Context.Set<TModel>();
        }

        public IQueryable<TModel> FindAllBy(System.Linq.Expressions.Expression<Func<TModel, bool>> where)
        {
            return Context.Set<TModel>().Where(where.Compile()).AsQueryable();
        }

        public TModel FindOneBy(System.Linq.Expressions.Expression<Func<TModel, bool>> where)
        {
            return Context.Set<TModel>().FirstOrDefault(where.Compile());
        }

        public async Task<IQueryable<TModel>> FindAllAsync()
        {
            return await Task.Run<IQueryable<TModel>>(() =>
            {
                return Context.Set<TModel>();
            });
        }

        public async Task<IQueryable<TModel>> FindAllByAsync(System.Linq.Expressions.Expression<Func<TModel, bool>> where)
        {
            return await Task.Run<IQueryable<TModel>>(() =>
            {
                return FindAllBy(where);
            });
        }

        public async Task<TModel> FindOneByAsync(System.Linq.Expressions.Expression<Func<TModel, bool>> where)
        {
            return await Task.Run<TModel>(() =>
            {
                return FindOneBy(where);
            });
        }
    }
}
