using ConferenceAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.Core
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();

        IRepository<T> GetRepository<T>() where T : class;
    }
}
