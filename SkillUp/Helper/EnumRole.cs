﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillUp.Helper
{
    public static class EnumRole
    {
        public static string Instructor { get; set; } = "Instructor";
        public static string Admin { get; set; } = "Admin";
        public static bool IsInstructor { get; set; } = false;
    }
}
