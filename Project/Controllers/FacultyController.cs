using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Dal;
using Project.Models;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Project.Controllers
{
    public class FacultyController : Controller
    {
        // GET: Faculty
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cours_Registration()
        {
            CoursesDal dal = new CoursesDal();
            List<string> courses = new List<string>();
            foreach(CourseModel x in dal.Courses)
            {
                courses.Add(x.CourseID.ToString());
            }

            ViewBag.courses = courses;
            return View("Cours_Registration");
        }

        public ActionResult AddStudent(CourseAssignModel c)
        {
            //Check if student exist in Users Table as student
            UserDal usersD = new UserDal();
            List<UserModel> UserL = (from us in usersD.users
                                     where (us.UserID.Contains(c.id) & us.Type.Contains("student"))
                                     select us).ToList<UserModel>();
            if (UserL.Count < 1)
                return View("RegistrationError");


            //if student exist, Get scheduale of course from courses table
            CoursesDal Cdal = new CoursesDal();
            List<CourseModel> list = (from x in Cdal.Courses
                                      where x.CourseID.Contains(c.course)
                                      select x).ToList<CourseModel>();
            string day = list[0].Day1;
            TimeSpan hour = list[0].SHour1;
            string class1 = list[0].Class;

            //check if the student already assign to another course in this time. 
            Student_coursesDal dal2 = new Student_coursesDal();
            List<Student_Course_Model> list2 = (from y in dal2.StudentAndCourses
                                                where (y.StudentID.Contains(c.id)
                                                & y.Day.Contains(day) & y.Hour.Equals(hour))
                                                select y).ToList<Student_Course_Model>();

            if (list2.Count == 0)
            {
                Student_Course_Model newAssign = new Student_Course_Model
                {
                    StudentID = c.id,
                    FirstName = c.Fname,
                    LastName = c.Lname,
                    courseID = c.course,
                    Day = day,
                    Hour = hour,
                    Class = class1,
                    GradeA = -1,
                    GradeB = -1,
                    FinalGrade = -1
                };

                dal2.StudentAndCourses.Add(newAssign);
                dal2.SaveChanges();
                return View("RegistedSuccessfully"); 
            }
            else
                return View("RegistrationError");
        }

        public ActionResult EditOfGrade()
        {
            CoursesDal dal = new CoursesDal();
            List<string> courses = new List<string>();
            foreach (CourseModel x in dal.Courses)
            {
                courses.Add(x.CourseID.ToString());
            }

            ViewBag.courses = courses;
            return View("Change_grades");
        }

        public ActionResult ChangeGrades(string course, string id)
        {
            //check if student assign to this course
            Session["C"] = course;
            Session["id"] = id;
            Student_coursesDal dal = new Student_coursesDal();

            List<Student_Course_Model> student = (from x in dal.StudentAndCourses
                                                  where (x.StudentID.Contains(id) & x.courseID.Contains(course))
                                                  select x).ToList<Student_Course_Model>();
            if (student.Count <1 )
            {
                return View("ErrorUpdateGrades");
            }

            ViewBag.MoedA = student[0].GradeA;
            ViewBag.MoedB = student[0].GradeB;
            ViewBag.Final= student[0].FinalGrade;
            ViewBag.st = student[0].StudentID;
            ViewBag.c = course;
            ViewBag.studentInCourse = student[0];

            return View("GradeTable");
               
        }

        public ActionResult EditG(int Text1,int Text2, int final)
        {

            string id = Session["id"].ToString();
            string course = Session["c"].ToString();

            if ((Text1 < -1 | Text1 > 100) | (Text2 < -1 | Text2 > 100) | (final < -1 | final > 100))
                return View("ErrorUpdateGrades");
           
            SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-RIUF1NMK;database=Moodle;Integrated Security=SSPI");
            con.Open();
            SqlCommand sqlcomm = new SqlCommand(); 
            sqlcomm.CommandText = "update [CurseStudent] set GradeB= '" + Text2 + "', GradeA= '" + Text1 + "', FinalGrade= '" + final + "' where StudentID= '" + id + "' and CourseID= '" + course + "'";
            sqlcomm.Connection = con;
            sqlcomm.ExecuteNonQuery();

            con.Close();
            return View("ChangedSuccessfully");
           
        }


        public ActionResult EditSchedualeForm ()
        {
            CoursesDal dal = new CoursesDal();

            ViewBag.AllCourses = dal.Courses;
            return View("EditSchedualeForm");
        }

        [HttpPost]
        public ActionResult EditSchedule(string courseId, string day, TimeSpan shour, TimeSpan ehour, string Class)
        {
            if (isClashing(day, shour, ehour))
                return View("UpdateScheFailed");
            
            SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-RIUF1NMK;database=Moodle;Integrated Security=SSPI");
            con.Open();
            SqlCommand sqlcomm = new SqlCommand();
            //Update courses table
            sqlcomm.CommandText = "update [Courses] set Day1= '" + day + "', SHour1= '" + shour + "',Class = '" +Class+"' where CourseID= '" + courseId + "'";
            //Update studentCourses table
            sqlcomm.Connection = con;
            sqlcomm.ExecuteNonQuery();
            con.Close();

            //SqlConnection con1 = new SqlConnection(@"Data Source=LAPTOP-RIUF1NMK;database=Moodle;Integrated Security=SSPI");
            con.Open();
            SqlCommand sqlcomm1 = new SqlCommand();
            sqlcomm1.CommandText = "update [CurseStudent] set Day='" + day + "', Hour= '" + shour + "', Class= '" + Class + "' where CourseID= '" + courseId + "'";
            sqlcomm1.Connection = con;
            sqlcomm1.ExecuteNonQuery();
            con.Close();

            return View("EditSuccessfully");
        }

        public bool isClashing(string day, TimeSpan shour, TimeSpan ehour)
        {
            CoursesDal dal = new CoursesDal();
            List<CourseModel> coursesToCheck = (from x in dal.Courses
                                                where x.Day1.Contains(day)
                                                select x).ToList<CourseModel>(); 
           int i = 0;
            while (i < coursesToCheck.Count())
            {
  
                bool a = shour <= coursesToCheck[i].SHour1 && ehour <= coursesToCheck[i].Ehour1 &&
                    ehour > coursesToCheck[i].SHour1;
                bool b = shour > coursesToCheck[i].SHour1 && ehour < coursesToCheck[i].Ehour1;
                bool c = shour < coursesToCheck[i].SHour1 && ehour >= coursesToCheck[i].Ehour1;
                bool d = shour > coursesToCheck[i].SHour1 && ehour > coursesToCheck[i].Ehour1 &&  shour < coursesToCheck[i].Ehour1;
                if (a || b || c || d)
                    return true;            
                
                i++;
            }
            return false;
        }

        public ActionResult ExamsEditForm ()
        {
            ExamsDal dal = new ExamsDal();
            CoursesDal dal2 = new CoursesDal();

            ViewBag.AllCourses = dal2.Courses;
            ViewBag.exams = dal.exams;
            return View("ExamsForm");
        }

        //This is a help function fore edit exam schedule - check wheter specipic date is already exist. 
        public bool DateExist(int day, int month, int year)
        {
            ExamsDal dal = new ExamsDal();
            List<ExamModel> examsThisDate = (from x in dal.exams
                                             where (x.Day.Equals(day) & x.Month.Equals(month) & x.Year.Equals(year))
                                             select x).ToList<ExamModel>();
            if (examsThisDate.Count > 0)
                return true;
            else
                return false;
        }

        [HttpPost]
        public ActionResult EditScheduleExam(string courseId, string moed, string Class, DateTime date, TimeSpan hour)
        {
            int day = date.Day;
            int month = date.Month;
            int year = date.Year;

            ExamsDal dal1 = new ExamsDal();
            ExamModel e = dal1.exams.Find(courseId, moed);
            var newdate = date.Date;

            ExamsDal dal = new ExamsDal();
            List<ExamModel> examsThisDate = (from x in dal.exams
                                             where (x.Day.Equals(day) & x.Month.Equals(month) & x.Year.Equals(year))
                                             select x).ToList<ExamModel>();

            //check if its Moed B - that it's date is after moed A
            if (moed=="B")
            {
               ExamModel examInMoedA = dal1.exams.Find(courseId, "A");
               DateTime examAdate = new DateTime(examInMoedA.Year, examInMoedA.Month, examInMoedA.Day);
               int res = DateTime.Compare(examAdate,date);
               if (res>=0)
                {
                    return View("EditExamScheError");
                }
            }


            //There is already a date for this exam, than we need to update exist entity 
            if (e != null)
            {
                if (examsThisDate.Count > 0)
                {
                    //There is already exam in this date for another course
                    if (examsThisDate[0].CourseID != courseId)
                    {
                        return View("EditExamScheError");
                    }
                    //Maybe faculty administrator wants to update just time or class 
                    else
                    {
                        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-RIUF1NMK;database=Moodle;Integrated Security=SSPI");
                        con.Open();
                        SqlCommand sqlcomm = new SqlCommand();
                        sqlcomm.CommandText = "update [Exams] set Day='" + day + "', Month- '" + month + "', Year= '" + year + "', Hour='" + hour + "', Class = '" + Class + "' where CourseID= '" + courseId + "' and Moed='" + moed + "'";
                        sqlcomm.Connection = con;
                        sqlcomm.ExecuteNonQuery();
                        con.Close();

                    }
                    return View("ChangedSuccessfully");
                }
                //There isn't exams in this date 
                else
                {
                    SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-RIUF1NMK;database=Moodle;Integrated Security=SSPI");
                    con.Open();
                    SqlCommand sqlcomm1 = new SqlCommand();
                    sqlcomm1.CommandText = "update [Exams] set Day='" + day + "', Month= '" + month + "', Year= '" + year + "', Hour='" + hour + "', Class = '" + Class + "' where CourseID= '" + courseId + "' and Moed='" + moed + "'";
                    sqlcomm1.Connection = con;
                    sqlcomm1.ExecuteNonQuery(); 
                    con.Close();
                    return View("ChangedSuccessfully");
                }
            }

            //There isn't a date for this exam yet, we need to create a new entitiy 
            else
            {
                if (examsThisDate.Count == 0)
                {
                    ExamModel newExam = new ExamModel
                    {
                        CourseID = courseId,
                        Moed = moed,
                        Day = day,
                        Year = year,
                        Month = month,
                        Hour = hour,
                        Class = Class
                    };

                    dal1.exams.Add(newExam);
                    dal1.SaveChanges();
                    return View("ChangedSuccessfully");

                }
              
            }
            return View("EditExamScheError");
        }

        public ActionResult Add_Course_Form ()
        {
            //create a list of all Lecturer in college from Users DB
            UserDal dal = new UserDal();
            List<UserModel> lecturers = (from x in dal.users
                                         where x.Type.Contains("lecturer")
                                         select x).ToList<UserModel>();

            ViewBag.lecturers = lecturers;
            return View("NewCourseForm");

        }

      public ActionResult AddNewCourse(string lecturer,string lecturerId, string course, string day, TimeSpan shour, TimeSpan ehour, string Class)
      {
            //check this course isn't already exist , and times given
            CoursesDal dal = new CoursesDal();
            CourseModel TheCourse = dal.Courses.Find(course);
            if (TheCourse != null | isClashing(day, shour, ehour))
                return View("AddingCourseError");

            //create new Course and add it to DB
            CourseModel newCourse = new CourseModel
            {CourseID = course, LecturerName = lecturer, LecturerID= lecturerId, Day1 = day, SHour1 = shour,Ehour1 = ehour, Class=Class};

            dal.Courses.Add(newCourse);
            dal.SaveChanges();
            return View("AddSuccessfully");
      }
    }
}
