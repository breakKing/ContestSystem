using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Models
{
    public class ContestSystemDbContext: IdentityDbContext<IdentityUser>
    {
        public ContestSystemDbContext(DbContextOptions<ContestSystemDbContext> options): base(options)
        {
            Database.EnsureCreated();
            DefaultInit();
        }

        private void DefaultInit()
        {
            
        }
    }
}
