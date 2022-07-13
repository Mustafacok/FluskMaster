using FluskMasterUpdate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FluskMasterUpdate.Controllers
{
    public class BlogsController : Controller
    {
        // GET: Blog
        BlogsBLL _blogs = new BlogsBLL();
        public ActionResult Index(string Adi="")
        {
            var liste = _blogs.GetAll();
            var model = liste.FirstOrDefault(f => f.SEOLink == Adi);
            return View();
        }
    }
}