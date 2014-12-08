using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Signup()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View(); 
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message;
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View(); 


        }

        public ActionResult ViewRandom()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}