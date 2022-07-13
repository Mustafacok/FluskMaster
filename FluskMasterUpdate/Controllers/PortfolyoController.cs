using FluskMasterUpdate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FluskMasterUpdate.Controllers
{
    public class PortfolyoController : Controller
    {
        // GET: Portfolyo
        PortfolyoBLL _portfolyo = new PortfolyoBLL();
        public ActionResult Index(string Adi="")
        {
            var liste = _portfolyo.GetAll();
            var model = liste.FirstOrDefault(f => f.SEOLink == Adi);
            return View();
        }
    }
}