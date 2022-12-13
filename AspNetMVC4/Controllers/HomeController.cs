using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetMVC4.Controllers
{
   public class HomeController : Controller
    {
        [CustomAuthorizeAttribute]
        public ActionResult Index()
        {
            bool flag = User.Identity.IsAuthenticated;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}