using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Accuracy.Core.Repositories
{
    public interface IRepository<TModel> where TModel : class
    {
        void Save(TModel instance);
        void Update(TModel instance);
        void Delete(TModel instance);
        Task SaveAsync(TModel instance);
        Task UpdateAsync(TModel instance);
        Task DeleteAsync(TModel instance);

        IQueryable<TModel> FindAll();
        IQueryable<TModel> FindAllBy(Expression<Func<TModel, bool>> where);
        TModel FindOneBy(Expression<Func<TModel, bool>> where);

        Task<IQueryable<TModel>> FindAllAsync();
        Task<IQueryable<TModel>> FindAllByAsync(Expression<Func<TModel, bool>> where);
        Task<TModel> FindOneByAsync(Expression<Func<TModel, bool>> where);
    }
}
