 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Dal;
using Project.Models;
using System.Data.SqlClient;
using System.Data;
namespace Project.Controllers
{
    public class LecturerController : Controller
    {
        // GET: Lecturer
        public ActionResult Index(string submit)
        {
            Response.Write(submit);
            return View();
        }

        ActionResult Index()
        {
            return View();
        }

        public ActionResult Watch_scheduale()
        {
            string lecturerId = Session["UserID"].ToString();

            CoursesDal dal = new CoursesDal();

            List<CourseModel> Courses = (from x in dal.Courses
                                         where x.LecturerID.Contains(lecturerId)
                                         select x).ToList<CourseModel>();

          
            ViewBag.CoursesList = Courses;
            return View("Watch_scheduale");
        }


        public ActionResult Watch_Courses()
        {
            string loggedLecturer = Session["UserID"].ToString();
            string name = Session["UserName"].ToString();
            CoursesDal dal1 = new CoursesDal();

            List<CourseModel> RelevantCourses = (from x in dal1.Courses
                                                 where (x.LecturerID.Contains(name))
                                                 select x).ToList<CourseModel>();

            ViewBag.ReleventCourses = RelevantCourses;
            return View("Watch_Courses");
        }

        public ActionResult Choose_Course()
        {
            string loggedLecturer = Session["UserID"].ToString();
            string name = Session["UserName"].ToString();
            CoursesDal dal1 = new CoursesDal();

            List<CourseModel> RelevantCourses = (from x in dal1.Courses
                                                 where (x.LecturerID.Contains(loggedLecturer))
                                                 select x).ToList<CourseModel>();

            ViewBag.ReleventCourses = RelevantCourses;
            return View("Choose_Course");
        }

        public ActionResult enter(string courseId)
        {
            Student_coursesDal dal1 = new Student_coursesDal();
            UserDal users_dal = new UserDal();

            List<Student_Course_Model> studentsInCourse = (from x in dal1.StudentAndCourses
                                                           where (x.courseID.Contains(courseId))
                                                           select x).ToList<Student_Course_Model>();

            ViewBag.Students_courses = studentsInCourse;

            return View("List_Of_Students");
        }

        public ActionResult Edit_Grades()
        {
            string loggedLecturer = Session["UserID"].ToString();

            CoursesDal dal1 = new CoursesDal();

            List<CourseModel> RelevantCourses = (from x in dal1.Courses
                                                 where (x.LecturerID.Contains(loggedLecturer))
                                                 select x).ToList<CourseModel>();

            ViewBag.ReleventCourses = RelevantCourses;
            return View("Edit_Grades");
        }

        [HttpPost]
        public ActionResult CanEditGradesCheck(string Course, string Moed)
        {
            DateTime cuurentDate = DateTime.Today;
            ExamsDal examsdal = new ExamsDal();
            ExamModel exam = examsdal.exams.Find(Course, Moed);

            //There isn't a date for this exam
            if (exam == null)
                return View("FailedUpdateGrades");

            //Student in course Bag:
            Student_coursesDal dal = new Student_coursesDal();

            List<Student_Course_Model> lis = (from x in dal.StudentAndCourses
                                              where x.courseID.Contains(Course)
                                              select x).ToList<Student_Course_Model>();
            ViewBag.studentInCourse = lis;
            Session["CourseTOupdate"] = Course;
            Session["Moed"] = Moed;
            //check if exam date already pass: 
            DateTime examDate = new DateTime(exam.Year, exam.Month, exam.Day);
            int flag = DateTime.Compare(cuurentDate, examDate);
            if (flag <= 0)
            {
                return View("FailedUpdateGrades");
            }

            else
            {
                ViewBag.MoedOfUpdate = Moed;
                return View("SubmitGradeChanges");
            }

        }

        public ActionResult SubmitGrades (string studentId, int grade)
        {
            if (grade < -1 | grade > 100)
                return View("GradeError");

            /////////////////////////////////////////////////////////////////

            string moed = Session["Moed"].ToString();
            string Course = Session["CourseTOupdate"].ToString();
            int finalGrade;

            Student_coursesDal dal = new Student_coursesDal();
            Student_Course_Model student = dal.StudentAndCourses.Find(studentId, Course);

            //If it's Moed B, we need to update final grade:
            //-1 for someone that doesn't exam in moed B
            if (moed=="B")
            {
                if (grade == (-1))
                    finalGrade = student.GradeA;
                else
                    finalGrade = grade;
            }
            else
                finalGrade =-1;

            //Update DB with the grade 
            SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-RIUF1NMK;database=Moodle;Integrated Security=SSPI");
            con.Open();
            SqlCommand sqlcomm = new SqlCommand();

            //two options - moed A and moed B 
            if (moed.Equals("A"))
                sqlcomm.CommandText = "update [CurseStudent] set GradeA= '" + grade + "', FinalGrade = '"+ finalGrade + "' where StudentID= '" + studentId + "' and CourseID= '" + Course + "'";
            else
            {
                
                sqlcomm.CommandText = "update [CurseStudent] set GradeB= '" + grade + "' , FinalGrade = '" + finalGrade + "' where StudentID= '" + studentId + "' and CourseID= '" + Course + "'";
            }
            sqlcomm.Connection = con;
            sqlcomm.ExecuteNonQuery();
            con.Close();

            return View("GradesUpdatedSuccessfully");
        }

    }
}