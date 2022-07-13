using FluskMasterUpdate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FluskMasterUpdate.Controllers
{
    public class ReferanslarController : Controller
    {
        // GET: Portfolyo
        ReferansBLL _referans = new ReferansBLL();
        public ActionResult Index(string Adi="")
        {
            var liste = _referans.GetAll();
            var model = liste.FirstOrDefault(f => f.SEOLink == Adi);
            return View();
        }
    }
}