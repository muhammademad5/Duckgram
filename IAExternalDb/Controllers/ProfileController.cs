using IAExternalDb.Context;
using IAExternalDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IAExternalDb.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        DataContext db = new DataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserProfile(int id)
        {
            User user = db.UserTable.ToList().SingleOrDefault(x => x.user_id == id);
            User CurrentUser = Session["CurrentUser"] as User;
            ViewBag.AlreadyRequested = checkifAlreadySendRequest(id, CurrentUser.user_id);
            ViewBag.AlreadyFriend = checkifAlreadyFriend(id, CurrentUser.user_id);
            if (user.user_id == CurrentUser.user_id)
            {
                return RedirectToAction("myProfile");
            }
            return View(user);
        }
        [HttpGet]
        public ActionResult myProfile()
        {
            User CurrentLoggedin = Session["CurrentUser"] as IAExternalDb.Models.User;
            User user = db.UserTable.ToList().SingleOrDefault(x => x.user_id == CurrentLoggedin.user_id);
            return View(user);
        }
        [HttpGet]
        public ActionResult EditProfile()
        {
            User CurrentLoggedin = Session["CurrentUser"] as IAExternalDb.Models.User;
            User user = db.UserTable.ToList().SingleOrDefault(x => x.user_id == CurrentLoggedin.user_id);
            return View(user);
        }
        [HttpPost]
        public ActionResult EditProfile(User user, HttpPostedFileBase file)
        {
            User CurrentLoggedin = Session["CurrentUser"] as IAExternalDb.Models.User;
            var St = db.UserTable.SingleOrDefault(x => x.user_id == CurrentLoggedin.user_id);
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/ProfilePictures/"), pic);

                file.SaveAs(path);

                St.ProfilePicture = pic;
            }
            St.Fname = user.Fname;
            St.Lname = user.Lname;
            St.Telephone = user.Telephone;
            St.Password = user.Password;

            db.Entry(St).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("myProfile");
        }
        [HttpGet]
        public ActionResult ShowFollowers(int id)
        {
            return View(db.UserTable.ToList().SingleOrDefault(x => x.user_id == id).FollowersList.ToList());
        }
        [HttpGet]
        public ActionResult ShowFollowing(int id)
        {
            return View(db.UserTable.ToList().SingleOrDefault(x => x.user_id == id).FollowingList.ToList());
        }

        [HttpGet]
        public int addRequest(int id)
        {   
            User CurrentUser = Session["CurrentUser"] as User;
            User toFollow = db.UserTable.SingleOrDefault(x => x.user_id == id);
            string sql = "insert into UserRequests values("+id+","+CurrentUser.user_id+" )";
            db.Database.ExecuteSqlCommand(sql);
            db.SaveChanges();
            
            return 1;
        }
        public int delRequest(int id)
        {
            User CurrentUser = Session["CurrentUser"] as User;
            //User toFollow = db.UserTable.SingleOrDefault(x => x.user_id == id);
            string sql = "delete from UserRequests where(UserId=" + id+ "and RequesterID="+ CurrentUser.user_id+") ";
            db.Database.ExecuteSqlCommand(sql);
            db.SaveChanges();

            return 1;
        }
        [HttpPost]
        public ActionResult SearchResult()
        {
            string s = HttpContext.Request.Form["username"];
            List<User> SearchResult = new List<User>();
            List<User> Result = new List<User>();
            foreach (var user in db.UserTable.ToList())
            {
                if (user.Fname.Contains(s)|| user.Lname.Contains(s)||user.Email==s)
                {
                    Result.Add(user);
                }
            }
            return View(Result);
        }
        public int delFriend_Request(int id)
        {
            User CurrentUser = Session["CurrentUser"] as User;
            //User toFollow = db.UserTable.SingleOrDefault(x => x.user_id == id);
            string sql = "delete from UserRequests where(UserId=" + CurrentUser.user_id + "and RequesterID=" + id + ") ";
            db.Database.ExecuteSqlCommand(sql);
            db.SaveChanges();

            return 1;
        }

        public int accFriend_Request(int id)
        {
            User CurrentUser = Session["CurrentUser"] as User;
            string sql = "insert into UserFollower values("+CurrentUser.user_id +","+ id+") ";
            string sql2 = "insert into UserFollowing values(" + id + "," + CurrentUser.user_id + ") ";
            string sql3 = "delete from UserRequests where(UserId=" + CurrentUser.user_id + "and RequesterID=" + id + ") ";
            db.Database.ExecuteSqlCommand(sql);
            db.Database.ExecuteSqlCommand(sql2);
            db.Database.ExecuteSqlCommand(sql3);
            db.SaveChanges();

            return 1;
        }

        public bool checkifAlreadySendRequest(int userid,int meid)
        {
            foreach(var entry in db.UserTable.SingleOrDefault(x => x.user_id == userid).FriendsReqList)
            {
                if (entry.user_id == meid)
                {
                    return true;
                }
            }
            return false;
        }

        public bool checkifAlreadyFriend(int userid, int meid)
        {
            foreach(var entry in db.UserTable.SingleOrDefault(x => x.user_id == userid).FollowersList)
            {
                if (entry.user_id == meid)
                {
                    return true;
                }
            }
            return false;
        }
    }
}