using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluskMasterUpdate.Models;

namespace FluskMasterUpdate.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        BlogsBLL _blogs = new BlogsBLL();
        public ActionResult Index()
        {
            return View(_blogs.Get(Size:8));
        }
        public ActionResult Header()
        {
            return PartialView();
        }
        public ActionResult Footer()
        {
            return PartialView();
        }
    }
}