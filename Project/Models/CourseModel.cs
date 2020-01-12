using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    [Table("Courses")]
    public class CourseModel
    {
        [Key, Column(Order = 0)]
        public string CourseID { get; set;}
        
        public string LecturerID { get; set; }

        public string LecturerName { get; set; }
        public string Day1 { get; set; }
   
        public TimeSpan SHour1 { get; set; }
        public TimeSpan Ehour1 { get; set; }
        public string Class { get; set; }
    }
}