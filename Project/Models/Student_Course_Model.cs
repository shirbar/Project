using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    [Table("CurseStudent")]
    public class Student_Course_Model
    {
        [Required]
        [Key,Column(Order =0)]
        public string StudentID { set; get; }
        [Key,Column(Order = 1)]
        public string courseID { set; get; }
        public int GradeA {set; get;}
        public int GradeB { set; get; }
        public string Day { get; set; }
        public string Hour { get; set; }
        public string Class { get; set; }
    }
}