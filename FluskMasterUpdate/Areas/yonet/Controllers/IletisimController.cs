using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluskMasterUpdate.Models;
using FluskMasterUpdate.m2;

namespace FluskMasterUpdate.Areas.yonet.Controllers
{
    public class IletisimController : Controller
    {
        // GET: yonet/Iletisim
        IletisimBLL _iletisim = new IletisimBLL();
        public ActionResult Index(string q="")
        {
            int count = 0;
            var model = _iletisim.Get(out count, g=>g.Adi.Contains(q));
            ViewBag.Count = count;
            return View(model);
        }
        public ActionResult IndexDetay(string term="")
        {
            int count = 0;
            var model = _iletisim.Get(out count, g => g.Adi.Contains(term));
            ViewBag.Count = count;
            return PartialView(model);
        }
        public ActionResult AramaYap(string term)
        {
            var model = _iletisim.Get(g => g.Adi.Contains(term));
            var result = model.Select(s => new { adi = s.Adi, id = s.Id }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult KayitSil(string cbSecili = "")
        {
            if (cbSecili !="")
            {
                var parcala = cbSecili.Split(',').Select(Int32.Parse).ToList();
                foreach (var item in parcala)
                {
                    _iletisim.Delete(item);
                }
            }
            return RedirectToAction("Index","Iletisim");
        }
        public ActionResult Detay(int Id=0)
        {
            var model = _iletisim.GetById(Id);
            ViewBag.Mesaj = GenelAraclarBLL.KayitYeni();
            return View(model);
        }

        [HttpPost]
        public ActionResult Detay(Iletisim model)
        {
            try
            {
                _iletisim.InsertOrUpdate(model, model.Id);
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