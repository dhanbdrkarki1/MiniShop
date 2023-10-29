﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // T - Category/model
        IEnumerable<T> GetAll(string? includeProperties = null);

        // for linked operation
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        void Remove (T entity);
        void RemoveRange(IEnumerable<T> entity);

    }
}