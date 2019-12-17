using ConferenceAPI.Core;
using ConferenceAPI.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Dictionary<Type, object>();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            try
            {
                _dbContext.Set<T>();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Entity of type " + typeof(T).ToString() + " doesn't exist in dbContext");
            }

            if (!_repositories.TryGetValue(typeof(T), out var repository))
            {
                repository = new Repository<T>(_dbContext);
                _repositories.Add(typeof(T), repository);
            }

            return repository as IRepository<T>;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
