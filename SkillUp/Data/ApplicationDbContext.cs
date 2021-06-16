using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillUp.EF;
using SkillUp.Models;

namespace SkillUp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseStudent> CourseStudents { get; set; }
    }
}
