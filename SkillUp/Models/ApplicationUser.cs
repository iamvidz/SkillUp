using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillUp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string PicturePath { get; set; }

    }
}
