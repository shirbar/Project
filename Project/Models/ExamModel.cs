using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    [Table("Exams")]
    public class ExamModel
    {
        [Required]
        [Key, Column(Order = 0)]
        public string CourseID { get; set; }
        [Key, Column(Order = 1)]
        public string Moed {get; set;}
        public string Date { get; set; }
        public string Hour { get; set; }
        public string Class { get; set; }

    }
}