using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluskMasterUpdate.Models;
using FluskMasterUpdate.m2;

namespace FluskMasterUpdate.Areas.yonet.Controllers
{
    public class BlogsController : Controller
    {
        // GET: yonet/Blogs
        BlogsBLL _blogs = new BlogsBLL();
        public ActionResult Index(string q="")
        {
            int count = 0;
            var model = _blogs.Get(out count, g=>g.Adi.Contains(q));
            ViewBag.Count = count;
            return View(model);
        }
        public ActionResult IndexDetay(string term="")
        {
            int count = 0;
            var model = _blogs.Get(out count, g=>g.Adi.Contains(term));
            ViewBag.Count = count;
            return PartialView(model);
        }
        public ActionResult AramaYap(string term)
        {
            var model = _blogs.Get(g=>g.Adi.Contains(term));
            var result = model.Select(s => new { adi = s.Adi, id = s.Id }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult KayitSil(string cbSecili="")
        {
            if (cbSecili !="")
            {
                var parcala = cbSecili.Split(',').Select(Int32.Parse).ToList();
                foreach (var item in parcala)
                {
                    var model = _blogs.GetById(item);
                    if (System.IO.File.Exists(HttpContext.Request.PhysicalApplicationPath + "yonet/images/blogs/" + model.Gorsel))
                    {
                        System.IO.File.Exists(HttpContext.Request.PhysicalApplicationPath + "yonet/images/blogs/" + model.Gorsel);
                    }

                    using (BlogsBLL del = new BlogsBLL())
                    {
                        del.Delete(item);
                    }
                }                
            }
            return RedirectToAction("Index","Blogs");
        }
        public ActionResult Detay(int Id=0)
        {
            var model = _blogs.GetById(Id);
            ViewBag.Mesaj = GenelAraclarBLL.KayitYeni();
            return View(model);
        }
        [HttpPost]
        public ActionResult Detay(Blogs model, HttpPostedFileBase fGorsel)
        {
            try
            {
                if (fGorsel != null && fGorsel.ContentLength >0)
                {
                    model.Gorsel = fGorsel.FileName;
                    fGorsel.SaveAs(HttpContext.Request.PhysicalApplicationPath + "yonet/images/blogs/" + fGorsel.FileName);
                }
                _blogs.InsertOrUpdate(model,model.Id);
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