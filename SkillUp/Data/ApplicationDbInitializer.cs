using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillUp.Data
{
    public class ApplicationDbInitializer
    {
        public static async Task InitializeIdentity(ApplicationDbContext db,RoleManager<IdentityRole> roleManager)
        {
            db.Database.EnsureCreated();
            //create role admin if it's not created yet
            var role = await roleManager.FindByNameAsync(Helper.EnumRole.Admin);
            if(role!=null)
            {
                return;//db has been seeded
            }
            string[] roles = { Helper.EnumRole.Instructor, Helper.EnumRole.Admin };
            foreach(var rname in roles)
            {
                role = new IdentityRole(rname);
                await roleManager.CreateAsync(role);
            }
        }
    }
}
