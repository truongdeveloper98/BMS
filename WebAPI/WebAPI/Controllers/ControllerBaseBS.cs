using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ControllerBaseBS : ControllerBase
    {
        protected readonly UsageDbContext _db;

        public ControllerBaseBS(UsageDbContext context)
        {
            _db = context;
        }

    }
}
