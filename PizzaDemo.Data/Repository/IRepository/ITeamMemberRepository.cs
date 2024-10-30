﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaDemo.Models;

namespace PizzaDemo.DataAccess.Repository.IRepository
{
    public interface ITeamMemberRepository : IRepository<TeamMember>
    {
        void Update(TeamMember obj);
    }
}