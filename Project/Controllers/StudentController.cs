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

            CoursesDal courseDal = new CoursesDal();
            List<CourseModel> coursesList = new List<CourseModel>();
            CourseModel helpCourse; 

            foreach(Student_Course_Model x in ourList)
            {
                helpCourse = courseDal.Courses.Find(x.courseID);
                if(helpCourse!=null)
                    coursesList.Add(helpCourse);
            }
                
            ViewBag.CoursesList = coursesList;                                             
            return View("Watch_scheduale");
        }

        public ActionResult Watch_Exams()
        {
            string LoggedUser = Session["UserID"].ToString();
            Student_coursesDal dal = new Student_coursesDal();
            //Export from data courses table all the courses of logged user
            //list of Student_courses_model
            List<Student_Course_Model> Student_Courses_list = (from x in dal.StudentAndCourses
                                                            where x.StudentID.Contains(LoggedUser)
                                                            select x).ToList<Student_Course_Model>();
            
            //Create a list of Exams
            ExamsDal examDal = new ExamsDal();
            List<ExamModel> examsList = new List<ExamModel>();
            ExamModel HelpExamA, HelpExamB;

            foreach(Student_Course_Model x in Student_Courses_list)
            {
                string name = x.courseID;
            }

            foreach(Student_Course_Model x in Student_Courses_list)
            {
                HelpExamA = examDal.exams.Find(x.courseID,"A");
                HelpExamB = examDal.exams.Find(x.courseID, "B");
                if (HelpExamA != null)
                    examsList.Add(HelpExamA);
                if(HelpExamB != null)
                    examsList.Add(HelpExamB);
            }

            ViewBag.ExamsList = examsList;
            return View("Watch_Exams");
        }

        public ActionResult Watch_Grades()
        {
            string LoggedUser = Session["UserID"].ToString();
            Student_coursesDal dal = new Student_coursesDal();
   
            List<Student_Course_Model> Student_Courses_list = (from x in dal.StudentAndCourses
                                                               where x.StudentID.Contains(LoggedUser)
                                                               select x).ToList<Student_Course_Model>();

            List<Student_Course_Model> Help = new List<Student_Course_Model>();
           foreach (Student_Course_Model x in Student_Courses_list)
            {
                if (x.GradeA != -1 & x.GradeB != -1)
                    Help.Add(x);
            }
            ViewBag.grades = Help;
            return View("Watch_Grades");

        }
    }
}