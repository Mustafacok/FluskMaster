using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluskMasterUpdate.Models;
using FluskMasterUpdate.m2;


namespace FluskMasterUpdate.Areas.yonet.Controllers
{
    public class ReferansController : Controller
    {
        // GET: yonet/Referans
        ReferansBLL _referans = new ReferansBLL();
        public ActionResult Index(string q = "")
        {
            int count = 0;
            var model = _referans.Get(out count, g => g.AdiSoyadi.Contains(q));
            ViewBag.Count = count;
            return View(model);
        }
        public ActionResult IndexDetay(string term = "")
        {
            int count = 0;
            var model = _referans.Get(out count, g => g.AdiSoyadi.Contains(term));
            ViewBag.Count = count;
            return PartialView(model);
        }
        public ActionResult AramaYap(string term)
        {
            var model = _referans.Get(g => g.AdiSoyadi.Contains(term));
            var result = model.Select(s => new { adisoyadi = s.AdiSoyadi, id = s.Id }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult KayitSil(string cbSecili = "")
        {
            if (cbSecili != "")
            {
                var parcala = cbSecili.Split(',').Select(Int32.Parse).ToList();
                foreach (var item in parcala)
                {
                    var model = _referans.GetById(item);
                    if (System.IO.File.Exists(HttpContext.Request.PhysicalApplicationPath + "yonet/images/referans/" + model.Gorsel))
                    {
                        System.IO.File.Delete(HttpContext.Request.PhysicalApplicationPath + "yonet/images/referans/" + model.Gorsel);
                    }
                    //_referans.Delete(item); //db

                    // hem dosyayı hem db kayıt silme
                    using (ReferansBLL del = new ReferansBLL())
                    {
                        del.Delete(item);
                    }
                }
            }
            return RedirectToAction("Index", "Referans");
        }
        public ActionResult Detay(int Id = 0)
        {
            var model = _referans.GetById(Id);
            ViewBag.Mesaj = GenelAraclarBLL.KayitYeni();
            return View(model);
        }
        [HttpPost]
        public ActionResult Detay(Referans model, HttpPostedFileBase fGorsel)
        {
            try
            {
                if (fGorsel != null && fGorsel.ContentLength > 0)
                {
                    model.Gorsel = fGorsel.FileName;
                    fGorsel.SaveAs(HttpContext.Request.PhysicalApplicationPath + "yonet/images/referans/" + fGorsel.FileName);
                }
                _referans.InsertOrUpdate(model, model.Id);
                ViewBag.Mesaj = GenelAraclarBLL.KayitBasarili();
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Mesaj = GenelAraclarBLL.KayitHatali(mesaj: ex.Message);
                return View(model);
            }
        }
    }
}