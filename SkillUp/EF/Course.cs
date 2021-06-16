using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SkillUp.EF
{
    public class Course
    {
        public Course()
        {
            CourseStudents = new HashSet<CourseStudent>();
        }
        public Guid CourseId { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [StringLength(50)]
        public string WistiaId { get; set; }
        public long Price { get; set; }
        [Required]
        [StringLength(450)]
        public string TeacherId { get; set; }
        [StringLength(450)]
        public string ImagePath { get; set; }
        public long Revenue { get; set; }
        public virtual Models.ApplicationUser Teacher { get; set; }
        public virtual ICollection<CourseStudent> CourseStudents { get; set; }
    }
}
