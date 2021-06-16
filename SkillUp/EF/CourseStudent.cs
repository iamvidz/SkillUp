using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SkillUp.EF
{
    public class CourseStudent
    {
        public Guid CourseStudentId { get; set; }
        [Required]
        public Guid CourseId { get; set; }
        [Required]
        [StringLength(450)]
        public string StudentId { get; set; }
        public virtual Models.ApplicationUser Student { get; set; }
        public virtual Course Course { get; set; }
    }
}
