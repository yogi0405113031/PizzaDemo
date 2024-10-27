using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaDemo.DataAccess.Data;
using PizzaDemo.DataAccess.Repository.IRepository;
using PizzaDemo.Models;

namespace PizzaDemo.DataAccess.Repository
{
    public class StoreRepository : Repository<Store>, IStoreRepository
    {
        public StoreRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update(Store obj)
        {
            _db.Stores.Update(obj);
        }
    }
}
