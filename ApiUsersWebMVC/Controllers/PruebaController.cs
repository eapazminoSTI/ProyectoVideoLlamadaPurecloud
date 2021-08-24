using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApiUsersWebMVC.Controllers
{
    public class PruebaController : Controller
    {
        // GET: Prueba
        public ActionResult Test()
        {
            return View();
        }

        public JsonResult Edit(int? id)
        {
            id = 3;

            if (id == null)
            {
                return Json(new { success = false, message = "Mensaje ERROR" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, message = "Mensaje OK" }, JsonRequestBehavior.AllowGet);
        }
    }
}