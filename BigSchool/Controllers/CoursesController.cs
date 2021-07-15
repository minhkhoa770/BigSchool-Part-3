using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        BigSchoolDB context = new BigSchoolDB();
        // GET: Courses
        public ActionResult Create()
        {
            BigSchoolDB context = new BigSchoolDB();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();

            return View(objCourse);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            BigSchoolDB context = new BigSchoolDB();
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid) 
            {
                objCourse.ListCategory = context.Categories.ToList();
                return View("Create", objCourse);
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LectureId = user.Id;
            context.Courses.Add(objCourse);
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Attending()
        {
            BigSchoolDB context = new BigSchoolDB();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var course = new List<Course>();
            foreach(Attendance temp in listAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.LectureName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                    .FindById(objCourse.LectureId).Name;
                course.Add(objCourse);
            }
            return View(course);
        }

        public ActionResult Mine()
        {
            BigSchoolDB context = new BigSchoolDB();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = context.Courses.Where(c => c.LectureId == currentUser.Id && c.Datetime > DateTime.Now).ToList();

            foreach (Course i in courses)
            {
                i.LectureName = currentUser.Name;
            }
            return View(courses);
        }

        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser =
            System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
            .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            var listFollwee = context.Followings.Where(p => p.FollowerId == currentUser.Id).ToList();

            var listAttendances = context.Attendances.Where(p => p.Attendee ==

            currentUser.Id).ToList();

            var courses = new List<Course>();
            foreach (var course in listAttendances)

            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LectureId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName =
                        System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LectureId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }



    }
}