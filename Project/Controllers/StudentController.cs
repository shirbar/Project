using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Dal;
using Project.Models;
using System.Configuration;
using System.Data;

namespace Project.Controllers
{
    public class StudentController : Controller
    {
        public ActionResult Watch_scheduale()
        {
            string studentID1 = Session["UserID"].ToString();

            Student_coursesDal dal = new Student_coursesDal();

            List<Student_Course_Model> ourList = (from x 
                                                  in dal.StudentAndCourses
                                                  where x.StudentID.Contains(studentID1)
                                                  select x).ToList<Student_Course_Model>();
            ViewBag.CoursesList = ourList;                                             
            return View("Watch_scheduale");
        }

        public ActionResult Watch_Exams()
        {
            string LoggedUser = Session["UserID"].ToString();
            Student_coursesDal dal = new Student_coursesDal();
            //Export from data courses table all the courses of logged user
            //list of Student_courses_model
            List<Student_Course_Model> Student_Courses_list = (from x
                                                 in dal.StudentAndCourses
                                                  where x.StudentID.Contains(LoggedUser)
                                                  select x).ToList<Student_Course_Model>();

            //From the list above create a list of courses
            List<string> Courseslist = new List<string>();

            foreach(Student_Course_Model X in Student_Courses_list)
                { Courseslist.Add((X.courseID).ToString()); }


            //create list of Exams
            ExamsDal dal2 = new ExamsDal();
            List<ExamModel> ExamsList = new List<ExamModel>(); 
            List<ExamModel> HelpList = new List<ExamModel>(); //temp list 

            foreach (string Course in Courseslist)
            {
                HelpList = ((from x in dal2.exams
                               where x.CourseID.Contains(Course)
                               select x).ToList<ExamModel>());

                ExamsList.AddRange(HelpList);
            }

            ViewBag.ExamsList = ExamsList;
            return View("Watch_Exams");
        }

        public ActionResult Watch_Grades()
        {
            string LoggedUser = Session["UserID"].ToString();
            Student_coursesDal dal = new Student_coursesDal();
   
            List<Student_Course_Model> Student_Courses_list = (from x in dal.StudentAndCourses
                                                               where x.StudentID.Contains(LoggedUser)
                                                               select x).ToList<Student_Course_Model>();
            ViewBag.grades = Student_Courses_list;
            return View("Watch_Grades");

        }
    }
}