using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaDemo.Models;

namespace PizzaDemo.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        //意味著 CategoryRepository 可以被讀取，但不能從接口外部被設置
        IProductRepository Product { get; }
        IStoreRepository Store { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderHeaderRepository OrderHeader { get; }
        ITeamMemberRepository TeamMember { get; }
        void Save();
    }
}
