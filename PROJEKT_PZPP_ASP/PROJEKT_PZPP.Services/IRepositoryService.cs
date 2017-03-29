using PROJEKT_PZPP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PROJEKT_PZPP.Services
{
    interface IRepositoryService<T>  where T : class
    {
  
        IQueryable<T> GetAll();
        T GetSingle(int id);
        IQueryable<T> FindBy(Expression<Func<T,bool>> where);
        bool Edit(T entity);
        bool Add(T entity);
        bool Delete(T entity);
        bool Save();

    }

    public class RepositoryService<T> : IRepositoryService<T> where T : class, IEntity<int>
    {
        protected DbContext _context;
        protected DbSet<T> _set;

        public RepositoryService(DbContext context)
        {
            _context = context;
            _set = (_context as DbContext).Set<T>();
        }

        public virtual bool Add(T entity)
        {
            bool result;
            try
            {
                _set.Add(entity);
                result = Save();
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        public virtual bool Delete(T entity)
        {
            bool result;
            try
            {
                _set.Remove(entity);
                result = Save();
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public virtual bool Edit(T entity)
        {
            bool result;
            try
            {
                (_context as DbContext).Entry(entity).State = System.Data.Entity.EntityState.Modified;
                result = Save();
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> where)
        {
            return _set.Where(where);
        }

        public virtual IQueryable<T> GetAll()
        {
            return _set;
        }

        public virtual T GetSingle(int id)
        {

            var result = _set.FirstOrDefault(r => r.Id == id);

            return result;
        }

        public virtual bool Save()
        {
            bool result;
            try
            {
                ((DbContext)_context).SaveChanges();
                result = true;
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;

        }
    }
}

