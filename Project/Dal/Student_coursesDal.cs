using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Project.Models;

namespace Project.Dal
{
    public class Student_coursesDal : DbContext
    {
  
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<Student_Course_Model>().ToTable("CurseStudent");
            }
            public DbSet <Student_Course_Model> StudentAndCourses { get; set; }

    }
}