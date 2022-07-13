using FluskMasterUpdate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FluskMasterUpdate.Controllers
{
    public class IletisimController : Controller
    {
        // GET: Iletisim
        IletisimBLL _iletisim = new IletisimBLL();
        public ActionResult Index(string Adi="")
        {
            var liste = _iletisim.GetAll();
            var model = liste.FirstOrDefault(f =>f.SEOLink == Adi);
            return View();
        }
    }
}