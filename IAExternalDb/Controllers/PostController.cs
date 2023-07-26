using IAExternalDb.Context;
using IAExternalDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IAExternalDb.Controllers
{
    public class PostController : Controller
    {
        // GET: Post
        DataContext db = new DataContext();
        public ActionResult Index()
        {
            return View();
        }
        /* [HttpGet]
         public int Like(int indexofimage)
         {
             User user = db.UserTable.ToList().SingleOrDefault(x => x.user_id == Int32.Parse(Session["CurrentUser"].ToString()));
             Like newLike = new Like()
             {
                 Post_ID = user.PostsList.ToList()[indexofimage].id,
                 like_owner_id = user.user_id
             };
             db.LikeTable.Add(newLike);
             db.SaveChanges();
             return user.PostsList.ToList()[indexofimage].LikesList.Count;
         }*/
        [HttpGet]
        public int Like (int postID)
        {
            User CurrentUser = Session["CurrentUser"] as User;
            Like newLike = new Like()
            {
                Post_ID = postID,
                like_owner_id = CurrentUser.user_id
            };
            db.LikeTable.Add(newLike);
            db.SaveChanges();
            return db.PostTable.ToList().SingleOrDefault(x => x.id == postID).LikesList.Count;
        }
        [HttpGet]
        public int DisLike(int postID)
        {
            User CurrentUser = Session["CurrentUser"] as User;
            Dislike newDisLike = new Dislike()
            {
                Post_ID = postID,
                dislike_owner_id = CurrentUser.user_id
            };
            db.DislikeTable.Add(newDisLike);
            db.SaveChanges();
            return db.PostTable.ToList().SingleOrDefault(x => x.id == postID).DislikesList.Count;
        }
        [HttpGet]
        public int DelLike(int postID)
        {
            User CurrentUser = Session["CurrentUser"] as User;
            Like willbeDeletedLike = db.LikeTable.SingleOrDefault(x => x.Post_ID == postID && x.like_owner_id == CurrentUser.user_id);
            db.LikeTable.Remove(willbeDeletedLike);
            db.SaveChanges();
            return db.PostTable.ToList().SingleOrDefault(x => x.id == postID).LikesList.Count;
        }
        [HttpGet]
        public int DelDisLike(int postID)
        {
            User CurrentUser = Session["CurrentUser"] as User;
            Dislike willbeDeletedLike = db.DislikeTable.SingleOrDefault(x => x.Post_ID == postID && x.dislike_owner_id == CurrentUser.user_id);
            db.DislikeTable.Remove(willbeDeletedLike);
            db.SaveChanges();
            return db.PostTable.ToList().SingleOrDefault(x => x.id == postID).DislikesList.Count;
        }

        public ActionResult showPost(int id) // id of post
        {
            User CurrentUser = Session["CurrentUser"] as User;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserLiked = CheckIfUserLiked(id, CurrentUser.user_id);
            ViewBag.CurrentUserDisLiked = CheckIfUserDisLiked(id, CurrentUser.user_id);
            System.Diagnostics.Debug.WriteLine(CheckIfUserDisLiked(id, CurrentUser.user_id));
            return View(db.PostTable.SingleOrDefault(item => item.id == id));
        }
        [HttpGet]
        public string postComment(int postID,string CommentText)
        {
            User CurrentUser = Session["CurrentUser"] as User;
            Comment newComment = new Comment()
            {
                comment_owner_id = CurrentUser.user_id,
                Post_ID = postID,
                text = CommentText,
                owner_name = CurrentUser.Fname
            };
            db.CommentTable.Add(newComment);
            db.SaveChanges();
            return CurrentUser.Fname;
        }

        [HttpGet]
        public ActionResult addPost()
        {
            return View();
        }

        [HttpPost]
        public ActionResult addPost(Post newPost,HttpPostedFileBase file)
        {
            User CurrentUser = Session["CurrentUser"] as User;
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Posts/"), pic);

                file.SaveAs(path);

                newPost.PostImage = pic;
            }
            newPost.Owner_Id = CurrentUser.user_id;
            db.PostTable.Add(newPost);
            db.SaveChanges();
            return RedirectToAction("myProfile", "Profile");
        }

        public bool CheckIfUserLiked(int postid,int userid)
        {
            foreach (var entry in db.PostTable.SingleOrDefault(x => x.id == postid).LikesList.ToList())
            {
                if (entry.like_owner_id == userid)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckIfUserDisLiked(int postid, int userid)
        {
            foreach (var entry in db.PostTable.SingleOrDefault(x => x.id == postid).DislikesList.ToList())
            {
                if (entry.dislike_owner_id == userid)
                {
                    return true;
                }
            }
            return false;
        }
    }
}