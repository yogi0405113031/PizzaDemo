using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDemo.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    //where T : class 是一個類型約束，表示 T 必須是引用類型（類）
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        //T 傳回類型, Func<T, bool> 委託類型，表示一個接受 T 類型參數並返回 bool 的函數
        //用於過濾數據 EX:u => u.Id == 1
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
