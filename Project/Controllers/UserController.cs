using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;
using Project.Dal;
using System.Configuration;


namespace Project.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult HomePage()
        {
            return View("HomePage");
        }
 
        public ActionResult Submit(UserModel user)
        {
          
            UserDal dal = new UserDal();
            
            string name = user.FirstName.ToString();
            string ID = user.UserID.ToString();
           // string lname = user.LastName;
            string password = user.Password.ToString();

            List<UserModel> usersList =
                (from x in dal.users
                 where (x.FirstName.Equals(name) & x.UserID.Equals(ID) & x.Password.Equals(password))
                 select x).ToList<UserModel>();

            TempData["loggedUser"] = usersList[0].UserID;
            //  ViewBag.logged = usersList[0].UserID;
            Session["UserID"] = ID;
            Session["UserName"] = name; //first name 
                                        // Session["LastName"] = lname; //last name 
            if (usersList.Count > 0)
                if (usersList[0].Type.Contains("student"))
                    return View("StudentPage");
                else if (usersList[0].Type.Contains("lecturer"))
                    return View("LecturerPage");
                else
                {
                    if (usersList[0].Type.Contains("faculty"))
                        return View("FacultyPage");
                    else
                        return View("ErrorPage");
                }
            else
                return View("ErrorPage");
        }
    }

    
}