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
    public class TeamMemberRepository : Repository<TeamMember>, ITeamMemberRepository
    {
        public TeamMemberRepository(ApplicationDbContext db) : base(db)
        {
        }


        public void Update(TeamMember obj)
        {
            var objFromDb = _db.TeamMembers.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Introduction = obj.Introduction;
                objFromDb.Position = obj.Position;
                objFromDb.ImageUrl = obj.ImageUrl;
            }
        }
    }
}
